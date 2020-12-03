using MathNet.Numerics.LinearAlgebra;
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

        private TrajectoryGenerator generator;

        private bool isInitialized = false;

        private bool forceMoveMode = false;

        private long IPOC;

        private E6POS position;

        private E6AXIS axisPosition;

        /// <summary>
        /// Robot Ip adress (RSI interface)
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
        public E6POS HomePosition { get; private set; }

        /// <summary>
        /// Robot current position
        /// </summary>
        public E6POS Position {
            get {
                lock (receivedDataSyncLock) {
                    return position;
                }
            }
        }

        /// <summary>
        /// Robot current axis position
        /// </summary>
        public E6AXIS AxisPosition {
            get {
                lock (receivedDataSyncLock) {
                    return axisPosition;
                }
            }
        }

        /// <summary>
        /// Robot current velocity
        /// </summary>
        public Vector<double> Velocity {
            get {
                return generator.Velocity;
            }
        }

        /// <summary>
        /// Robot current acceleration
        /// </summary>
        public Vector<double> Acceleration {
            get {
                //return generator.Acceleration;
                return null;
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
                generator = new TrajectoryGenerator(receivedFrame.Position);

                lock (receivedDataSyncLock) {
                    IPOC = receivedFrame.IPOC;
                    position = receivedFrame.Position;
                    HomePosition = receivedFrame.Position;
                }

                // Send first response (prevent connection timeout)
                rsiAdapter.SendData(new OutputFrame() {
                    Correction = new E6POS(),
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

            //TODO: ewentualnie sprawdzenie predkosci a1 - a6

            lock (receivedDataSyncLock) {
                IPOC = receivedFrame.IPOC;
                position = receivedFrame.Position;
                axisPosition = receivedFrame.AxisPosition;
            }

            if (!Limits.CheckAxisPosition(axisPosition)) {
                Uninitialize();
                throw new InvalidOperationException($"Axis position limit has been exceeded:" +
                    $"{Environment.NewLine}{axisPosition}");
            }

            if (!Limits.CheckPosition(position)) {
                Uninitialize();
                throw new InvalidOperationException($"Available workspace limit has been exceeded:" +
                    $"{Environment.NewLine}{position}");
            }

            FrameReceived?.Invoke(receivedFrame);
        }

        /// <summary>
        /// Sends data (IPOC, correction) to the robot, raises <see cref="KUKARobot.FrameSent">FrameSent</see> event
        /// </summary>
        private void SendData() {
            E6POS correction;

            lock (forceMoveSyncLock) {
                correction = generator.GetNextCorrection(position);
            }

            correction = new E6POS(correction.X, correction.Y, correction.Z, 0, correction.B, correction.C);

            if (!Limits.CheckCorrection(correction)) {
                Uninitialize();
                throw new InvalidOperationException($"Correction limit has been exceeded:" +
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
        /// Moves the robot to specified position (Sets target position).
        /// </summary>
        /// <param name="targetPosition">target position</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void MoveTo(E6POS targetPosition, double targetDuration) {
            if (!isInitialized) {
                throw new InvalidOperationException("Robot is not initialized");
            }

            if (!Limits.CheckPosition(targetPosition)) {
                throw new ArgumentException($"Target position is outside the available workspace:" +
                    $"{Environment.NewLine}{targetPosition}");
            }

            lock (forceMoveSyncLock) {
                if (forceMoveMode) {
                    return;
                }
            }

            generator.SetTargetPosition(targetPosition, targetDuration);
        }

        /// <summary>
        /// Shifts robot by the specified delta position.
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        public void Shift(E6POS deltaPosition, double targetDuration) {
            MoveTo(Position + deltaPosition, targetDuration);
        }

        /// <summary>
        /// Moves robot to the specified position and blocks current thread until position is reached.
        /// Enables force move mode during the movement.
        /// </summary>
        /// <param name="targetPosition">target position</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        /// <param name="xyzTolerance">maximum absolute XYZ error between the target and current position</param>
        /// <param name="abcTolerance">maximum absolute ABC error between the target and current position</param>
        public void ForceMoveTo(E6POS targetPosition, double targetDuration) {
            Console.WriteLine(targetPosition);
            MoveTo(targetPosition, targetDuration);

            
            lock (forceMoveSyncLock) {
                forceMoveMode = true;
            }

            ManualResetEvent targetPositionReached = new ManualResetEvent(false);

            void checkPosition(InputFrame frameReceived) {
                if (generator.TargetPositionReached) {
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
        /// Shifts robot by the specified delta position and blocks current thread until new position is reached.
        /// Enables force move mode during the movement.
        /// </summary>
        /// <param name="deltaPosition">desired position change</param>
        /// <param name="targetDuration">desired movement duration in seconds</param>
        /// <param name="xyzTolerance">maximum absolute XYZ error between the target and current position</param>
        /// <param name="abcTolerance">maximum absolute ABC error between the target and current position</param>
        public void ForceShift(E6POS deltaPosition, double targetDuration) {
            ForceMoveTo(Position + deltaPosition, targetDuration);
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