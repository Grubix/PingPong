﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.KUKA {

    public class KUKARobot : IDevice {

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        private readonly RSIAdapter rsiAdapter;

        private readonly RobotLimits limits;

        private readonly BackgroundWorker worker;

        private readonly ManualResetEvent reachTargetPositionEvent = new ManualResetEvent(false);

        private readonly object robotDataSyncLock = new object();

        private readonly object targetPositionSyncLock = new object();

        private volatile bool isInitialized = false;

        private volatile bool forceMoveMode = false;

        private TrajectoryGenerator generator;

        private long currentIPOC;

        private E6POS currentPosition;

        private E6AXIS currentAxisPosition;

        private (E6POS position, double duration) targetPosition;

        public const double defaultMovementDuration = 10.0;

        /// <summary>
        /// Robot Ip adress (RSI)
        /// </summary>
        public string Ip {
            get {
                return rsiAdapter.Ip;
            }
        }

        /// <summary>
        /// Robot lower workspace point
        /// </summary>
        public E6POS LowerWorkspacePoint {
            get {
                return new E6POS(
                    limits.LimitX.min,
                    limits.LimitY.min,
                    limits.LimitZ.min,
                    currentPosition.A,
                    currentPosition.B,
                    currentPosition.C
                );
            }
        }

        /// <summary>
        /// Robot upper workspace point
        /// </summary>
        public E6POS UpperWorkspacePoint {
            get {
                lock (robotDataSyncLock) {
                    return new E6POS(
                        limits.LimitX.max,
                        limits.LimitY.max,
                        limits.LimitZ.max,
                        currentPosition.A,
                        currentPosition.B,
                        currentPosition.C
                    );
                }
            }
        }

        /// <summary>
        /// Robot current position
        /// </summary>
        public E6POS CurrentPosition {
            get {
                lock (robotDataSyncLock) {
                    return currentPosition;
                }
            }
        }

        /// <summary>
        /// Robot current axis position
        /// </summary>
        public E6AXIS CurrentAxisPosition {
            get {
                lock (robotDataSyncLock) {
                    return currentAxisPosition;
                }
            }
        }

        /// <summary>
        /// Robot target position
        /// </summary>
        public E6POS TargetPosition {
            get {
                lock (targetPositionSyncLock) {
                    return targetPosition.position;
                }
            }
        }

        /// <summary>
        /// Occurs when the robot is initialized (connection has been established)
        /// </summary>
        public event InitializedEventHandler Initialized;

        /// <summary>
        /// Occurs when <see cref="InputFrame"/> frame is received
        /// </summary>
        public event FrameReceivedEventHandler FrameReceived;

        /// <summary>
        /// Occurs when <see cref="OutputFrame"/> frame is sent
        /// </summary>
        public event FrameSentEventHandler FrameSent;

        /// <param name="port">Port defined in RSI_EthernetConfig.xml</param>
        /// <param name="robotLimits">robot limits</param>
        public KUKARobot(int port, RobotLimits robotLimits) {
            rsiAdapter = new RSIAdapter(port);
            limits = robotLimits;

            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            worker.DoWork += async (sender, args) => {
                // Connect with the robot
                InputFrame receivedFrame = await rsiAdapter.Connect();
                generator = new TrajectoryGenerator(receivedFrame.Position);

                lock (robotDataSyncLock) {
                    currentIPOC = receivedFrame.IPOC;
                    currentPosition = receivedFrame.Position;
                    targetPosition = (currentPosition, defaultMovementDuration);
                }

                // Send response (prevent connection timeout)
                rsiAdapter.SendData(new OutputFrame() {
                    Message = "PingPong",
                    Correction = new E6POS(),
                    IPOC = currentIPOC
                });

                isInitialized = true;
                Initialized?.Invoke();

                // Start loop for receiving and sending data
                while (!worker.CancellationPending) {
                    await ReceiveRobotDataAsync();
                    MoveToTargetPosition();
                }

                isInitialized = false;
                rsiAdapter.Disconnect();
            };
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises <see cref="KUKARobot.FrameRecived">FrameReceived</see> event
        /// </summary>
        private async Task ReceiveRobotDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();

            lock (robotDataSyncLock) {
                currentIPOC = receivedFrame.IPOC;
                currentPosition = receivedFrame.Position;
                currentAxisPosition = receivedFrame.AxisPosition;
            }

            if (limits.CheckAxisPosition(currentAxisPosition)) {
                if (currentPosition == targetPosition.position) {
                    reachTargetPositionEvent.Set();
                }

                FrameReceived?.Invoke(receivedFrame);
            } else {
                SendError("Axis position limit has been exceeded");
            }
        }

        /// <summary>
        /// Moves robot to the target position, raises <see cref="KUKARobot.FrameSent">FrameSent</see> event
        /// </summary>
        private void MoveToTargetPosition() {
            bool errorOccured = false;
            string errorMessage = "";
            E6POS correction = new E6POS();

            lock (targetPositionSyncLock) {
                if (limits.CheckPosition(targetPosition.position)) {
                    E6POS nextPosition = generator.GetNextPosition(
                        currentPosition,
                        targetPosition.position,
                        targetPosition.duration
                    );

                    E6POS nextCorrection = nextPosition - currentPosition;
                    nextCorrection = new E6POS(
                        nextCorrection.X,
                        nextCorrection.Y,
                        nextCorrection.Z,
                        nextCorrection.A,
                        -nextCorrection.B,
                        -nextCorrection.C
                    );

                    //TODO: ogarnac dlaczego dodanie magicznych dwoch minusow sprawia ze wszystko dziala (° ͜ʖ °)

                    if (limits.CheckCorrection(nextCorrection)) {
                        correction = nextCorrection;
                    } else {
                        errorOccured = true;
                        errorMessage = "Correction limit has been exceeded";
                    }
                } else {
                    errorOccured = true;
                    errorMessage = "Available workspace limit has been exceeded";
                }
            }

            if (errorOccured) {
                SendError(errorMessage);
            } else {
                OutputFrame outputFrame = new OutputFrame() {
                    Correction = correction,
                    IPOC = currentIPOC
                };

                rsiAdapter.SendData(outputFrame);
                FrameSent?.Invoke(outputFrame);
            }
        }

        /// <summary>
        /// Sends error message to the robot, throws exception
        /// </summary>
        /// <param name="errorMessage">error message</param>
        private void SendError(string errorMessage) {
            OutputFrame errorFrame = new OutputFrame() {
                Message = $"Error: {errorMessage}",
                Correction = new E6POS(),
                IPOC = currentIPOC
            };

            rsiAdapter.SendData(errorFrame);

            Uninitialize();
            throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Moves the robot to specified position (Sets target position)
        /// </summary>
        /// <param name="position">target position</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        public void MoveTo(E6POS position, double movementDuration = defaultMovementDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (movementDuration <= 0) {
                throw new ArgumentException("Duration value must be greater than 0");
            }

            lock (targetPositionSyncLock) {
                if (forceMoveMode) {
                    return;
                }

                targetPosition = (position, movementDuration);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        public void Shift(E6POS deltaPosition, double movementDuration = defaultMovementDuration) {
            MoveTo(TargetPosition + deltaPosition, movementDuration);
        }

        /// <summary>
        /// Moves robot to the specified position and blocks current thread until it is reached
        /// </summary>
        /// <param name="position">target position</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        public void ForceMoveTo(E6POS position, double movementDuration = defaultMovementDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (movementDuration <= 0) {
                throw new ArgumentException("Duration value must be greater than 0");
            }

            lock (targetPositionSyncLock) {
                if (forceMoveMode) {
                    return;
                }

                targetPosition = (position, movementDuration);
                forceMoveMode = true;
            }

            reachTargetPositionEvent.Reset();
            reachTargetPositionEvent.WaitOne();

            lock (targetPositionSyncLock) {
                forceMoveMode = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        public void ForceShift(E6POS deltaPosition, double movementDuration = defaultMovementDuration) {
            ForceMoveTo(TargetPosition + deltaPosition, movementDuration);
        }

        public void Initialize() {
            if (isInitialized) {
                return;
            }

            worker.RunWorkerAsync();
        }

        public void Uninitialize() {
            if (!worker.CancellationPending) {
                worker.CancelAsync();
            }
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public override string ToString() {
            return $"KUKA::{Ip}";
        }

    }
}