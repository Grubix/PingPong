using System;
using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {

    class KUKARobot : IDevice {

        private class Velocity : KUKAVector {

            public Velocity(E6POS previousPosition, E6POS currentPosition, double deltaTime) {
                X = (currentPosition.X - previousPosition.X) / deltaTime;
                Y = (currentPosition.Y - previousPosition.Y) / deltaTime;
                Z = (currentPosition.Z - previousPosition.Z) / deltaTime;
                A = (currentPosition.A - previousPosition.A) / deltaTime;
                B = (currentPosition.B - previousPosition.B) / deltaTime;
                C = (currentPosition.C - previousPosition.C) / deltaTime;
            }

        }

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

        private readonly TrajectoryGenerator trajectoryGenerator;

        private InputFrame lastReceivedFrame;

        private E6POS targetPosition;

        public WorkspaceLimit WorkspaceLimit { get; set; }

        public E6POS TargetPosition {
            get {
                return targetPosition;
            }
            set {
                if (!WorkspaceLimit.CheckPosition(value)) {
                    SendError("The available workspace limit has been exceeded");
                }

                targetPosition = value;
            }
        }

        public E6POS CurrentPosition {
            get {
                return lastReceivedFrame.Position;
            }
        }

        public KUKAVector CurrentVelocity { get; private set; }

        public double DeltaTime { get; private set; }

        public event InitializeEventHandler OnInitialize;

        public event FrameReceivedEventHandler OnFrameReceived;

        public event FrameSentEventHandler OnFrameSent;

        public delegate void InitializeEventHandler(InputFrame receivedFrame);

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        public KUKARobot(int port, WorkspaceLimit workspaceLimit) {
            rsiAdapter = new RSIAdapter(port);
            WorkspaceLimit = workspaceLimit;
            trajectoryGenerator = new TrajectoryGenerator();
            targetPosition = new E6POS();
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();
            DeltaTime = rsiAdapter.DeltaTime;

            lock (CurrentVelocity) {
                CurrentVelocity = new Velocity(CurrentPosition, receivedFrame.Position, DeltaTime);
            }

            lock (lastReceivedFrame) {
                lastReceivedFrame = receivedFrame;
            }

            OnFrameReceived?.Invoke(lastReceivedFrame);
        }

        /// <summary>
        /// Move robot to TargetPosition, raises OnFrameSent event
        /// </summary>
        private void MoveToTargetPosition() {
            OutputFrame outputFrame = new OutputFrame() {
                IPOC = lastReceivedFrame.IPOC
            };

            lock (TargetPosition) {
                E6POS newCorrection = trajectoryGenerator.GoToPoint(CurrentPosition, TargetPosition, 10.0);
                newCorrection = newCorrection.ClearABC();
                //TODO: ogarnac dlaczego bez wyzerowania ABC kuka robi wir miecza

                if (CheckCorrection(newCorrection)) {
                    outputFrame.Correction = newCorrection;
                    Console.WriteLine(newCorrection);
                    //rsiAdapter.SendData(outputFrame);
                } else {
                    SendError("Dzieki naszym obliczeniom KUKA prawie zrobila wir miecza. Najs.");
                }
            }

            OnFrameSent?.Invoke(outputFrame);
        }

        private bool CheckCorrection(E6POS correction) {
            bool CheckValue(double value, double min, double max) {
                if (value < min) {
                    return false;
                } else if (value > max) {
                    return false;
                }

                return true;
            }

            return
                CheckValue(correction.X, -1, 1) &&
                CheckValue(correction.Y, -1, 1) &&
                CheckValue(correction.Z, -1, 1) &&
                CheckValue(correction.A, -1, 1) &&
                CheckValue(correction.B, -1, 1) &&
                CheckValue(correction.C, -1, 1);
        }

        /// <summary>
        /// Stops the robot program execution, throws Exception
        /// </summary>
        public void SendError(string errorMessage) {
            rsiAdapter.SendData(new OutputFrame() {
                IPOC = lastReceivedFrame.IPOC,
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

                lock (this) {
                    isInitialized = true;
                    lastReceivedFrame = receivedFrame;
                    TargetPosition = CurrentPosition;

                    // Send response (prevent connection timeout)
                    rsiAdapter.SendData(new OutputFrame() {
                        IPOC = lastReceivedFrame.IPOC,
                        Correction = new E6POS()
                    });
                }

                OnInitialize?.Invoke(lastReceivedFrame);

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
