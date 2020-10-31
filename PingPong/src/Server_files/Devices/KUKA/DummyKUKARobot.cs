using System;
using System.ComponentModel;
using System.Threading;

namespace PingPong.KUKA {

    public class DummyKUKARobot : IDevice {

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        private readonly RobotLimits limits;

        private readonly BackgroundWorker worker;

        private readonly ManualResetEvent reachTargetPositionEvent = new ManualResetEvent(false);

        private readonly object robotDataSyncLock = new object();

        private readonly object targetPositionSyncLock = new object();

        private volatile bool isInitialized = false;

        private volatile bool forceMoveMode = false;

        private TrajectoryGenerator generator;

        private E6POS currentPosition;

        private (E6POS position, double duration) targetPosition;

        public const double defaultMovementDuration = 10.0;

        /// <summary>
        /// Robot IP adress
        /// </summary>
        public string IP {
            get {
                return "127.1.0.0";
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

        /// <param name="robotLimits">robot limits</param>
        public DummyKUKARobot(RobotLimits robotLimits) {
            limits = robotLimits;

            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (sender, args) => {
                currentPosition = new E6POS();
                targetPosition = (currentPosition, defaultMovementDuration);
                generator = new TrajectoryGenerator(CurrentPosition);

                isInitialized = true;
                Initialized?.Invoke();

                // Start loop for receiving and sending data
                while (!worker.CancellationPending) {
                    ReceiveAndSendData();
                }
            };
        }

        private void ReceiveAndSendData() {
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
                throw new InvalidOperationException(errorMessage);
            } else {
                FrameSent?.Invoke(null);

                Thread.Sleep(4);
                
                lock (robotDataSyncLock) {
                    currentPosition += correction;
                }

                if (currentPosition == targetPosition.position) {
                    reachTargetPositionEvent.Set();
                }

                FrameReceived?.Invoke(null);
            }
        }

        /// <summary>
        /// Moves the robot to specified position (Sets target position)
        /// </summary>
        /// <param name="position">target position</param>
        /// <param name="duration">desired movement duration in seconds</param>
        public void MoveTo(E6POS position, double duration = defaultMovementDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (duration <= 0) {
                throw new ArgumentException("Duration value must be greater than 0");
            }

            lock (targetPositionSyncLock) {
                if (forceMoveMode) {
                    return;
                }

                targetPosition = (position, duration);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="duration">desired movement duration in seconds</param>
        public void Shift(E6POS deltaPosition, double duration = defaultMovementDuration) {
            MoveTo(TargetPosition + deltaPosition, duration);
        }

        /// <summary>
        /// Moves robot to the specified position and blocks current thread until position is reached
        /// </summary>
        /// <param name="position">target position</param>
        /// <param name="duration">desired movement duration in seconds</param>
        public void ForceMoveTo(E6POS position, double duration = defaultMovementDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (duration <= 0) {
                throw new ArgumentException("Duration value must be greater than 0");
            }

            lock (targetPositionSyncLock) {
                if (forceMoveMode) {
                    return;
                }

                targetPosition = (position, duration);
                forceMoveMode = true;
            }

            ManualResetEvent mre = new ManualResetEvent(false);

            void checkPosition(InputFrame frame) {
                if (currentPosition == targetPosition.position) {
                    mre.Set();
                    FrameReceived -= checkPosition;
                }
            };

            FrameReceived += checkPosition;
            mre.WaitOne();

            //reachTargetPositionEvent.Reset();
            //reachTargetPositionEvent.WaitOne();

            lock (targetPositionSyncLock) {
                forceMoveMode = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="duration">desired movement duration in seconds</param>
        public void ForceShift(E6POS deltaPosition, double duration = defaultMovementDuration) {
            ForceMoveTo(TargetPosition + deltaPosition, duration);
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
            return $"KUKA::{IP}";
        }

    }
}