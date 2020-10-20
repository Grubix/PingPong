using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.KUKA {

    class KUKARobot : IDevice {

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        private readonly RSIAdapter rsiAdapter;

        private readonly RobotLimits limits;

        private readonly BackgroundWorker backgroundWorker;

        private readonly TrajectoryGenerator trajectoryGenerator;

        private readonly object robotDataSyncLock = new object();

        private readonly object targetPositionSyncLock = new object();

        private volatile bool isInitialized = false;

        private bool forceMoveMode = false;

        private long currentIPOC = -1;

        private E6POS currentPosition = new E6POS();

        private E6POS targetPosition = new E6POS();

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
        /// Robot target position
        /// </summary>
        public E6POS TargetPosition {
            get {
                lock (targetPositionSyncLock) {
                    return targetPosition;
                }
            }
            set {
                lock (targetPositionSyncLock) {
                    if (forceMoveMode) {
                        return;
                    }

                    targetPosition = value;
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
        /// <param name="robotLimits">robot limits (workspace points, max correction)</param>
        public KUKARobot(int port, RobotLimits robotLimits) {
            rsiAdapter = new RSIAdapter(port);
            limits = robotLimits;
            trajectoryGenerator = new TrajectoryGenerator(); //TODO: to mozna tworzyc po inicjalizacji i np. pobrac aktualne pozycje kuki

            backgroundWorker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += async (sender, args) => {
                // Connect with the robot
                InputFrame receivedFrame = await rsiAdapter.Connect();

                lock (robotDataSyncLock) {
                    currentIPOC = receivedFrame.IPOC;
                    currentPosition = receivedFrame.Position;
                    targetPosition = currentPosition;
                }

                // Send response (prevent connection timeout)
                rsiAdapter.SendData(new OutputFrame() {
                    Message = "PingPong",
                    Correction = new E6POS(),
                    IPOC = currentIPOC
                });

                Initialized?.Invoke();

                // Start loop for receiving and sending data
                while (!backgroundWorker.CancellationPending) {
                    await ReceiveDataAsync();
                    MoveToTargetPosition();
                }

                isInitialized = false;
                rsiAdapter.Disconnect();
            };
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();

            lock (robotDataSyncLock) {
                currentIPOC = receivedFrame.IPOC;
                currentPosition = receivedFrame.Position;
            }

            FrameReceived?.Invoke(receivedFrame);
        }

        /// <summary>
        /// Move robot to TargetPosition, raises OnFrameSent event
        /// </summary>
        private void MoveToTargetPosition() {
            string errorMessage = "";
            bool limitExceeded = false;
            E6POS correction = new E6POS();

            lock (targetPositionSyncLock) {
                if (limits.CheckPosition(targetPosition)) {
                    E6POS computedCorrection = trajectoryGenerator.GoToPoint(currentPosition, targetPosition, 10.0) - currentPosition;

                    if (limits.CheckCorrection(computedCorrection)) {
                        correction = computedCorrection;
                    } else {
                        limitExceeded = true;
                        errorMessage = "Correction limit has been exceeded";
                    }
                } else {
                    limitExceeded = true;
                    errorMessage = "The available workspace limit has been exceeded";
                }
            }

            OutputFrame outputFrame = new OutputFrame() {
                Message = limitExceeded ? $"Error: {errorMessage}" : "PingPong",
                Correction = correction,
                IPOC = currentIPOC
            };

            rsiAdapter.SendData(outputFrame);

            if (limitExceeded) {
                Uninitialize();
                throw new InvalidOperationException(errorMessage);
            }

            FrameSent?.Invoke(outputFrame);
        }

        /// <summary>
        /// Moves robot to the target postion and locks TargetPosition property.
        /// Current thread will be blocked until robot reach target position
        /// </summary>
        /// <param name="position">target position</param>
        public void ForceMoveToPosition(E6POS position) {
            if (!isInitialized) {
                throw new InvalidOperationException("Device is not initialized");
            }

            lock (targetPositionSyncLock) {
                targetPosition = position;
                forceMoveMode = true;
            }

            ManualResetEvent reachTargetPositionEvent = new ManualResetEvent(false);

            void checkPosition(InputFrame receivedFrame) {
                if (receivedFrame.Position == targetPosition) {
                    FrameReceived -= checkPosition;
                    reachTargetPositionEvent.Set();
                }
            }

            FrameReceived += checkPosition;
            reachTargetPositionEvent.WaitOne();

            lock (targetPositionSyncLock) {
                forceMoveMode = false;
            }
        }

        public void Initialize() {
            if (isInitialized) {
                return;
            }

            backgroundWorker.RunWorkerAsync();
        }

        public void Uninitialize() {
            if (!backgroundWorker.CancellationPending) {
                backgroundWorker.CancelAsync();
            }
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        //TODO: 
        private List<E6POS> GetCalibrationPoints(E6POS startPosition, E6POS endPosition, uint intermediatePoints) {
            List<E6POS> points = new List<E6POS>();
            uint totalPoints = 2 + intermediatePoints;

            E6POS deltaPosition = new E6POS(
                (endPosition.X - startPosition.X) / (intermediatePoints + 1),
                (endPosition.Y - startPosition.Y) / (intermediatePoints + 1),
                (endPosition.Z - startPosition.Z) / (intermediatePoints + 1),
                (endPosition.A - startPosition.A) / (intermediatePoints + 1),
                (endPosition.B - startPosition.B) / (intermediatePoints + 1),
                (endPosition.C - startPosition.C) / (intermediatePoints + 1)
            );

            for (int i = 0; i < totalPoints; i++) {
                points.Add(startPosition + new E6POS(
                    deltaPosition.X * i,
                    deltaPosition.Y * i,
                    deltaPosition.Z * i,
                    deltaPosition.A * i,
                    deltaPosition.B * i,
                    deltaPosition.C * i
                ));
            }

            return points;
        }

    }
}