using System;
using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {

    class KUKARobot : IDevice {

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

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

        public long CurrentIPOC {
            get {
                return lastReceivedFrame.IPOC;
            }
        }

        public event InitializeEventHandler OnInitialize;

        public event FrameReceivedEventHandler OnFrameReceived;

        public event FrameSentEventHandler OnFrameSent;

        public delegate void InitializeEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public delegate void FrameSentEventHandler(OutputFrame frameSent);

        public KUKARobot(int port) {
            rsiAdapter = new RSIAdapter(port);
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();

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
                IPOC = CurrentIPOC
            };

            lock (TargetPosition) {
                outputFrame.Position = (E6POS) TargetPosition.Clone();
                TargetPosition.Reset(); //DLA POZYCJI ABSOLUTNEJ WYWWALIĆ
            }

            rsiAdapter.SendData(outputFrame);
            OnFrameSent?.Invoke(outputFrame);
        }

        /// <summary>
        /// Stops the robot program execution, raises OnFrameSent event
        /// </summary>
        public void SendError(string errorMessage) {
            OutputFrame outputFrame = new OutputFrame() {
                Message = $"Error: {errorMessage}",
                Position = CurrentPosition,
                IPOC = CurrentIPOC
            };

            rsiAdapter.SendData(outputFrame);
            OnFrameSent?.Invoke(outputFrame);
        }

        /// <summary>
        /// Establish connection with the robot, raises OnFrameReceived, OnFrameSent and OnInitialize events
        /// </summary>
        public void Initialize() {
            if (isInitialized) {
                return;
            }

            Task.Run(async () => {
                InputFrame receivedFrame = await rsiAdapter.Connect();

                lock (lastReceivedFrame) {
                    lastReceivedFrame = receivedFrame;
                }

                OnFrameReceived?.Invoke(lastReceivedFrame);

                TargetPosition = new E6POS();
                //TargetPosition = (E6POS)CurrentPosition.Clone(); //DLA POZYCJI ABSOLUTNEJ
                MoveToTargetPosition();

                lock (this) {
                    isInitialized = true;
                }

                OnInitialize?.Invoke();

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
