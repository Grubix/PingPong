using System;
using System.Threading.Tasks;

namespace PingPong.KUKA {

    class KUKARobot : IDevice {

        private class Velocity : RobotVector {

            public Velocity() {
                X = Y = Z = A = B = C = 0;
            }

            public Velocity(E6POS previousPosition, E6POS currentPosition, double deltaTime)
            {
                X = (currentPosition.X - previousPosition.X) / deltaTime;
                Y = (currentPosition.Y - previousPosition.Y) / deltaTime;
                Z = (currentPosition.Z - previousPosition.Z) / deltaTime;
                A = (currentPosition.A - previousPosition.A) / deltaTime;
                B = (currentPosition.B - previousPosition.B) / deltaTime;
                C = (currentPosition.C - previousPosition.C) / deltaTime;
            }

        }

        private volatile bool isInitialized = false;

        private readonly object robotDataSyncLock = new object();

        private readonly object targetPositionSyncLock = new object();
        
        private readonly RSIAdapter rsiAdapter;

        private readonly RobotLimits limits;

        public TrajectoryGenerator trajectoryGenerator;

        private long currentIPOC;

        private double currentDeltaTime;
        public double CurrentDeltaTime {
            get {
                lock (robotDataSyncLock) {
                    return currentDeltaTime;
                }
            }
        }

        private E6POS currentPosition;
        public E6POS CurrentPosition {
            get {
                lock (robotDataSyncLock) {
                    return currentPosition;
                }
            }
        }

        private Velocity currentVelocity;
        public RobotVector CurrentVelocity {
            get {
                lock (robotDataSyncLock) {
                    return currentVelocity;
                }
            }
        }

        private E6POS targetPosition;
        public E6POS TargetPosition {
            get {
                lock (targetPositionSyncLock) {
                    return targetPosition;
                }
            }
            set {
                if (!limits.CheckPosition(value)) {
                    SendError("The available workspace limit has been exceeded");
                }

                lock (targetPositionSyncLock) {
                    targetPosition = value;
                }
            }
        }

        public event InitializedEventHandler Initialized;

        public event FrameReceivedEventHandler FrameReceived;

        public event FrameSentEventHandler FrameSent;

        public delegate void InitializedEventHandler(InputFrame receivedFrame);

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        public KUKARobot(int port, RobotLimits robotLimits) {
            rsiAdapter = new RSIAdapter(port);
            limits = robotLimits;
            trajectoryGenerator = new TrajectoryGenerator();
            currentPosition = new E6POS();
            currentVelocity = new Velocity();
            targetPosition = new E6POS();
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();
            E6POS previousPosition = currentPosition;

            lock (robotDataSyncLock) {
                currentIPOC = receivedFrame.IPOC;
                currentDeltaTime = rsiAdapter.DeltaTime;
                currentPosition = receivedFrame.Position;
            }

            currentVelocity = new Velocity(previousPosition, currentPosition, 0.004);
            FrameReceived?.Invoke(receivedFrame);
        }

        /// <summary>
        /// Move robot to TargetPosition, raises OnFrameSent event
        /// </summary>
        private void MoveToTargetPosition() {
            E6POS correction = trajectoryGenerator.GoToPoint(CurrentPosition, TargetPosition, 30.0);
            Console.WriteLine(trajectoryGenerator.timeToDest);
            correction = new E6POS(
                correction.X,
                correction.Y,
                correction.Z,
                0,
                0,
                correction.C
            );

            if (limits.CheckCorrection(correction)) {
                OutputFrame outputFrame = new OutputFrame() {
                    IPOC = currentIPOC,
                    Correction = correction
                };

                Console.WriteLine(correction);
                rsiAdapter.SendData(outputFrame);
                FrameSent?.Invoke(outputFrame);
            } else {
                SendError("Correction val err");
            }
        }

        /// <summary>
        /// Stops the robot program execution, throws Exception
        /// </summary>
        public void SendError(string errorMessage) {
            if(!isInitialized) {
                throw new Exception("Device is not initialized");
            }

            rsiAdapter.SendData(new OutputFrame() {
                IPOC = currentIPOC,
                Message = $"Error: {errorMessage}",
                Correction = new E6POS()
            });

            Uninitialize();
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Establish connection with the robot, raises OnInitialize event
        /// </summary>
        public void Initialize() {
            if (isInitialized) {
                return;
            }

            Task.Run(async () => {
                InputFrame receivedFrame = await rsiAdapter.Connect();
                isInitialized = true;

                lock (robotDataSyncLock) {
                    currentIPOC = receivedFrame.IPOC;
                    currentPosition = receivedFrame.Position;

                    // Send response (prevent connection timeout)
                    rsiAdapter.SendData(new OutputFrame() {
                        IPOC = currentIPOC,
                        Correction = new E6POS()
                    });
                }

                TargetPosition = CurrentPosition;
                Initialized?.Invoke(receivedFrame);

                // Start loop for receiving and sending data
                while (isInitialized) {
                    await ReceiveDataAsync();
                    MoveToTargetPosition();
                }
            });
        }

        public void Uninitialize() {
            isInitialized = false;
            rsiAdapter.Disconnect();
        }

        public bool IsInitialized() {
            return isInitialized;
        }

    }
}