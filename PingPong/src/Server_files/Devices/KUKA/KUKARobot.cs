using PingPong.Utils;
using System;
using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {

    class KUKARobot : IDevice {

        public class Velocity {

            public double X { get; }
            public double Y { get; }
            public double Z { get; }
            public double A { get; }
            public double B { get; }
            public double C { get; }

            public Velocity(E6POS previousPosition, E6POS currentPosition, double deltaTime) {
                if (deltaTime < 0) {
                    throw new ArgumentException("DeltaTime value cannot be negative");
                }

                if (deltaTime < 0.001) {
                    throw new ArgumentException($"DeltaTime value is too small: {deltaTime}");
                }

                X = (currentPosition.X - previousPosition.X) / deltaTime;
                Y = (currentPosition.Y - previousPosition.Y) / deltaTime;
                Z = (currentPosition.Z - previousPosition.Z) / deltaTime;
                A = (currentPosition.A - previousPosition.A) / deltaTime;
                B = (currentPosition.B - previousPosition.B) / deltaTime;
                C = (currentPosition.C - previousPosition.C) / deltaTime;
            }

        }

        private bool isInitialized = false;

        private readonly Timer timer;

        private readonly RSIAdapter rsiAdapter;

        private readonly TrajectoryGenerator trajectoryGenerator;

        private InputFrame lastReceivedFrame;

        private E6POS targetPosition;

        public E6POS TargetPosition {
            get {
                return targetPosition;
            }
            set {
                //TODO: Sprawdzenie czy pozycja nie wykracza poza dopuszczalny zakres ruchu
                if (false) {
                    SendError("The available workspace limit has been exceeded");
                    throw new Exception("");
                }

                targetPosition = value;
            }
        }

        public E6POS CurrentPosition {
            get {
                return lastReceivedFrame.Position;
            }
        }

        public Velocity CurrentVelocity { get; set; }

        public event InitializeEventHandler OnInitialize;

        public event FrameReceivedEventHandler OnFrameReceived;

        public event FrameSentEventHandler OnFrameSent;

        public delegate void InitializeEventHandler(InputFrame receivedFrame);

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        public KUKARobot(int port) {
            timer = new Timer();
            rsiAdapter = new RSIAdapter(port);
            trajectoryGenerator = new TrajectoryGenerator();
            targetPosition = new E6POS();
            CurrentVelocity = new Velocity(new E6POS(), new E6POS(), 1);
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();

            lock (lastReceivedFrame) {
                lastReceivedFrame = receivedFrame;
            }

            lock (CurrentVelocity) {
                CurrentVelocity = new Velocity(CurrentPosition, receivedFrame.Position, rsiAdapter.TimeDelta);
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

                if(CheckCorrection(newCorrection)) {
                    outputFrame.Correction = newCorrection;
                    Console.WriteLine(newCorrection);
                    //rsiAdapter.SendData(outputFrame);
                } else {
                    Uninitialize();
                    throw new Exception();
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
        /// Stops the robot program execution, raises OnFrameSent event
        /// </summary>
        public void SendError(string errorMessage) {
            OutputFrame outputFrame = new OutputFrame() {
                IPOC = lastReceivedFrame.IPOC,
                Message = $"Error: {errorMessage}",
                Correction = CurrentPosition
            };

            rsiAdapter.SendData(outputFrame);
            OnFrameSent?.Invoke(outputFrame);
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

                timer.Start();

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
