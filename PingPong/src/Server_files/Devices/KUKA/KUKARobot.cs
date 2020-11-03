using MathNet.Numerics.LinearAlgebra;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.KUKA {

    public class KUKARobot : IDevice {

        private readonly RSIAdapter rsiAdapter;

        private readonly RobotLimits limits;

        private readonly BackgroundWorker worker;

        private readonly object robotDataSyncLock = new object();

        private readonly object targetPositionSyncLock = new object();

        private bool isInitialized = false; //TODO: volatile ??

        private bool forceMoveMode = false; //TODO: volatile ??

        private TrajectoryGenerator generator;

        private long currentIPOC;

        private E6POS currentPosition;

        private E6AXIS currentAxisPosition;

        private (E6POS position, double duration) targetPosition;

        /// <summary>
        /// Robot Ip adress (RSI interface)
        /// </summary>
        public string Ip {
            get {
                return rsiAdapter.Ip;
            }
        }

        /// <summary>
        /// Robot lower workspace point
        /// </summary>
        public Vector<double> LowerWorkspacePoint {
            get {
                lock (robotDataSyncLock) {
                    return Vector<double>.Build.DenseOfArray(new double[] {
                        limits.LimitX.min,
                        limits.LimitY.min,
                        limits.LimitZ.min
                    });
                }
            }
        }

        /// <summary>
        /// Robot upper workspace point
        /// </summary>
        public Vector<double> UpperWorkspacePoint {
            get {
                lock (robotDataSyncLock) {
                    return Vector<double>.Build.DenseOfArray(new double[] {
                        limits.LimitX.max,
                        limits.LimitY.max,
                        limits.LimitZ.max
                    });
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
        public event Action Initialized; //TODO: EventHandler?

        /// <summary>
        /// Occurs when <see cref="InputFrame"/> frame is received
        /// </summary>
        public event Action<InputFrame> FrameReceived; //TODO: EventHandler?

        /// <summary>
        /// Occurs when <see cref="OutputFrame"/> frame is sent
        /// </summary>
        public event Action<OutputFrame> FrameSent; //TODO: EventHandler?

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
                    targetPosition = (currentPosition, 9999);
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
                    E6POS nextCorrection = generator.GetNextCorrection(
                        currentPosition,
                        targetPosition.position,
                        targetPosition.duration
                    );
                    
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
        public void MoveTo(E6POS position, double movementDuration) {
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
        /// Shifts robot by the specified delta position
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        public void Shift(E6POS deltaPosition, double movementDuration) {
            MoveTo(TargetPosition + deltaPosition, movementDuration);
        }

        /// <summary>
        /// Moves robot to the specified position and blocks current thread until position is reached
        /// </summary>
        /// <param name="position">target position</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        /// <param name="xyzTolerance">maximum absolute XYZ error between the target and current position</param>
        /// <param name="abcTolerance">maximum absolute ABC error between the target and current position</param>
        public void ForceMoveTo(E6POS position, double movementDuration, double xyzTolerance = 0.1, double abcTolerance = 0.1) {
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

            AutoResetEvent targetPositionReached = new AutoResetEvent(false);

            void checkPosition(InputFrame frameReceived) {
                if (currentPosition.Compare(targetPosition.position, xyzTolerance, abcTolerance)) {
                    targetPositionReached.Set();
                }
            };

            FrameReceived += checkPosition;
            targetPositionReached.WaitOne();
            FrameReceived -= checkPosition;

            lock (targetPositionSyncLock) {
                forceMoveMode = false;
            }
        }

        /// <summary>
        /// Shifts robot by the specified delta position and blocks current thread until new position is reached
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="movementDuration">desired movement duration in seconds</param>
        /// <param name="xyzTolerance">maximum absolute XYZ error between the target and current position</param>
        /// <param name="abcTolerance">maximum absolute ABC error between the target and current position</param>
        public void ForceShift(E6POS deltaPosition, double movementDuration, double xyzTolerance = 0.1, double abcTolerance = 0.1) {
            ForceMoveTo(TargetPosition + deltaPosition, movementDuration, xyzTolerance, abcTolerance);
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