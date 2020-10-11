using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {

    class KUKARobot : IDevice {

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

        private InputFrame lastReceivedFrame;

        public E6POS TargetPosition { get; set; }

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

            // On initialize start new thread for receiving and sending data
            OnInitialize += () => {
                Task.Run(async () => {
                    while (isInitialized) {
                        await ReceiveDataAsync();
                        MoveToTargetPosition();
                    }
                });
            };
        }

        /// <summary>
        /// Receives data from the robot asynchronously, raises OnFrameReceived event
        /// </summary>
        private async Task ReceiveDataAsync() {
            InputFrame receivedFrame = await rsiAdapter.ReceiveDataAsync();

            // Lock lastReceivedFrame object while assigning new data
            lock (lastReceivedFrame) {
                lastReceivedFrame = receivedFrame;
                OnFrameReceived?.Invoke(lastReceivedFrame);
            }
        }

        /// <summary>
        /// Move robot to TargetPosition, raises OnFrameSent event
        /// </summary>
        private void MoveToTargetPosition() {
            //TODO: Sprawdzenie czy pozycja jest dozwolona (nie wykracza poza dopuszczalny zakres ruchu)

            // Lock TargetPosition object while sending data to the robot
            lock (TargetPosition) {
                OutputFrame outputFrame = new OutputFrame() {
                    Position = TargetPosition,
                    IPOC = CurrentIPOC
                };

                rsiAdapter.SendData(outputFrame);
                OnFrameSent?.Invoke(outputFrame);
                TargetPosition.Reset(); //TODO: DLA POZYCJI ABSOLUTNEJ: WYWWALIĆ
            }
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
            if(isInitialized) {
                return;
            }

            Task.Run(async () => {
                InputFrame receivedFrame = await rsiAdapter.Connect();

                lock (lastReceivedFrame) {
                    lastReceivedFrame = receivedFrame;
                    OnFrameReceived?.Invoke(receivedFrame);
                }

                lock (TargetPosition) {
                    //TODO: DLA POZYCJI ABSOLUTNEJ: 
                    //TODO: TargetPosition = (E6POS) CurrentPosition.Clone()
                    TargetPosition = new E6POS();

                    OutputFrame outputFrame = new OutputFrame() {
                        Position = TargetPosition,
                        IPOC = CurrentIPOC
                    };

                    // Send response (prevent connection timeout)
                    rsiAdapter.SendData(outputFrame);
                    OnFrameSent?.Invoke(outputFrame);
                }

                lock (this) {
                    isInitialized = true;
                    OnInitialize?.Invoke();
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
