using PingPong.Maths;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.KUKA {

    public class KUKARobot : IDevice {

        private readonly object receivedDataSyncLock = new object();

        private readonly object forceMoveSyncLock = new object();

        private readonly BackgroundWorker worker;

        private readonly RSIAdapter rsiAdapter;

        private TrajectoryGenerator5 generator;

        private bool isInitialized = false;

        private bool forceMoveMode = false;

        private long IPOC; // timestamp

        private RobotVector position;

        private AxisPosition axisPosition;

        private RobotVector sumOfCorr = new RobotVector(0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Robot Ip adress (Robot Sensor Interface - RSI)
        /// </summary>
        public string Ip {
            get {
                return rsiAdapter.Ip;
            }
        }

        /// <summary>
        /// Robot limits
        /// </summary>
        public RobotLimits Limits { get; }

        /// <summary>
        /// Robot home position
        /// </summary>
        public RobotVector HomePosition { get; private set; }

        /// <summary>
        /// Robot current position
        /// </summary>
        public RobotVector Position {
            get {
                lock (receivedDataSyncLock) {
                    return position;
                }
            }
        }

        /// <summary>
        /// Robot current axis position
        /// </summary>
        public AxisPosition AxisPosition {
            get {
                lock (receivedDataSyncLock) {
                    return axisPosition;
                }
            }
        }

        /// <summary>
        /// Robot (theoretical) current velocity
        /// </summary>
        public RobotVector Velocity {
            get {
                return generator.Velocity;
            }
        }

        /// <summary>
        /// Robot (theoretical) current acceleration
        /// </summary>
        public RobotVector Acceleration {
            get {
                return generator.Acceleration;
            }
        }

        /// <summary>
        /// Robot current target position
        /// </summary>
        public RobotVector TargetPosition {
            get {
                return generator.TargetPosition;
            }
        }

        /// <summary>
        /// Flag that indicates if robot reached target position
        /// </summary>
        public bool IsTargetPositionReached {
            get {
                return generator.IsTargetPositionReached;
            }
        }

        /// <summary>
        /// Transformation from OptiTrack coordinate system to this robot coordinate system
        /// </summary>
        public Transformation OptiTrackTransformation { get; set; }

        /// <summary>
        /// Occurs when the robot is initialized (connection has been established)
        /// </summary>
        public event Action Initialized;

        /// <summary>
        /// Occurs when <see cref="InputFrame"/> frame is received
        /// </summary>
        public event Action<InputFrame> FrameReceived;

        /// <summary>
        /// Occurs when <see cref="OutputFrame"/> frame is sent
        /// </summary>
        public event Action<OutputFrame> FrameSent;

        /// <param name="port">Port defined in RSI_EthernetConfig.xml</param>
        /// <param name="limits">robot limits</param>
        public KUKARobot(int port, RobotLimits limits) {
            rsiAdapter = new RSIAdapter(port);
            Limits = limits;

            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            worker.DoWork += async (sender, args) => {
                // Connect with the robot
                InputFrame receivedFrame = await rsiAdapter.Connect();
                generator = new TrajectoryGenerator5(receivedFrame.Position);

                lock (receivedDataSyncLock) {
                    IPOC = receivedFrame.IPOC;
                    position = receivedFrame.Position;
                    HomePosition = receivedFrame.Position;
                }

                // Send first response (prevent connection timeout)
                rsiAdapter.SendData(new OutputFrame() {
                    Correction = new RobotVector(),
                    IPOC = IPOC
                });

                isInitialized = true;
                Initialized?.Invoke();

                // Start loop for receiving and sending data
                while (!worker.CancellationPending) {
                    await ReceiveDataAsync();
                    SendData();
                }

                isInitialized = false;
                rsiAdapter.Disconnect();
            };
        }

        /// <summary>
        /// Receives data (IPOC, cartesian and axis position) from the robot asynchronously, 
        /// raises <see cref="KUKARobot.FrameRecived">FrameReceived</see> event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();
            RobotVector correction = receivedFrame.Position - position;

            /*if (!Limits.CheckCorrection(correction)) {
                Uninitialize();
                throw new InvalidOperationException("Correction limit has been exceeded:" +
                    $"{Environment.NewLine}{correction}");
            }*/

            if (!Limits.CheckAxisPosition(receivedFrame.AxisPosition)) {
                Uninitialize();
                throw new InvalidOperationException("Axis position limit has been exceeded:" +
                    $"{Environment.NewLine}{receivedFrame.AxisPosition}");
            }

            if (!Limits.CheckPosition(receivedFrame.Position)) {
                Uninitialize();
                throw new InvalidOperationException("Available workspace limit has been exceeded:" +
                    $"{Environment.NewLine}{receivedFrame.Position}");
            }

            lock (receivedDataSyncLock) {
                IPOC = receivedFrame.IPOC;
                position = receivedFrame.Position;
                axisPosition = receivedFrame.AxisPosition;
            }

            FrameReceived?.Invoke(receivedFrame);
        }

        /// <summary>
        /// Sends data (IPOC, correction) to the robot, raises <see cref="KUKARobot.FrameSent">FrameSent</see> event
        /// </summary>
        private void SendData() {
            RobotVector correction;

            lock (forceMoveSyncLock) {
                correction = generator.GetNextCorrection(position);
            }

            //correction = new RobotVector(correction.X, correction.Y, correction.Z, 0, 0, 0);
            //sumOfCorr += correction;
            //Console.WriteLine(sumOfCorr);

            // CAŁY IF DO ZAKOMENTOWANIA DLA POZYCJI ABSOLUTNEJ
            if (!Limits.CheckCorrection(correction)) {
                Uninitialize();
                throw new InvalidOperationException("Correction limit has been exceeded:" +
                    $"{Environment.NewLine}{correction}");
            }

            OutputFrame outputFrame = new OutputFrame() {
                Correction = correction,
                IPOC = IPOC
            };

            rsiAdapter.SendData(outputFrame);
            FrameSent?.Invoke(outputFrame);
        }

        /// <summary>
        /// Moves robot to specified position (sets target position).
        /// </summary>
        /// <param name="targetPosition">target position</param>
        /// <param name="targetVelocity">target velocity (velocity after targetDuration)</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void MoveTo(RobotVector targetPosition, RobotVector targetVelocity, double targetDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (!Limits.CheckPosition(targetPosition)) {
                throw new ArgumentException("Target position is outside the available workspace:" +
                    $"{Environment.NewLine}{targetPosition}");
            }

            if (!Limits.CheckVelocity(targetVelocity)) {
                throw new ArgumentException("target velocity exceeding max value " +
                    $"({Limits.MaxVelocity.XYZ} [mm/s], {Limits.MaxVelocity.ABC} [deg/s]):" +
                    $"{Environment.NewLine}{targetVelocity}");
            }

            lock (forceMoveSyncLock) {
                if (forceMoveMode) {
                    return;
                }
            }

            generator.SetTargetPosition(targetPosition, targetVelocity, targetDuration);
        }

        /// <summary>
        /// Moves robot to the specified position and blocks current thread until position is reached
        /// Enables force move mode during the movement.
        /// </summary>
        /// <param name="targetPosition">target position</param>
        /// <param name="targetVelocity">target velocity (velocity after targetDuration)</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void ForceMoveTo(RobotVector targetPosition, RobotVector targetVelocity, double targetDuration) {
            MoveTo(targetPosition, targetVelocity, targetDuration);

            lock (forceMoveSyncLock) {
                forceMoveMode = true;
            }

            ManualResetEvent targetPositionReached = new ManualResetEvent(false);

            void checkPosition(InputFrame frameReceived) {
                if (generator.IsTargetPositionReached) {
                    targetPositionReached.Set();
                }
            };

            FrameReceived += checkPosition;
            targetPositionReached.WaitOne();
            FrameReceived -= checkPosition;

            lock (forceMoveSyncLock) {
                forceMoveMode = false;
            }
        }

        /// <summary>
        /// Shifts robot by the specified delta position
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="targetVelocity">target velocity (velocity after targetDuration)</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void Shift(RobotVector deltaPosition, RobotVector targetVelocity, double targetDuration) {
            MoveTo(Position + deltaPosition, targetVelocity, targetDuration);
        }

        /// <summary>
        /// Shifts robot by the specified delta position and blocks current thread until new position is reached.
        /// Enables force move mode during the movement.
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="targetVelocity">target velocity (velocity after targetDuration)</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void ForceShift(RobotVector deltaPosition, RobotVector targetVelocity, double targetDuration) {
            ForceMoveTo(Position + deltaPosition, targetVelocity, targetDuration);
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
            if (Ip != null) {
                return $"KUKA::{Ip}";
            } else {
                return "KUKA::[not initialized]";
            }
        }

    }
}