using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {

    class KUKARobot : IDevice {

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

        public InputFrame LastReceivedFrame { get; private set; }

        public OutputFrame LastFrameSent { get; private set; }

        public E6POS TargetPosition { get; set; } = new E6POS();

        public E6POS CurrentPosition {
            get {
                return LastReceivedFrame.Position;
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
            LastReceivedFrame = await rsiAdapter.ReceiveDataAsync();
            OnFrameReceived?.Invoke(LastReceivedFrame);
        }

        /// <summary>
        /// Move robot to TargetPosition, raises OnFrameSent event
        /// </summary>
        private void MoveToTargetPosition() {
            //TODO: Sprawdzenie czy pozycja jest dozwolona (nie wykracza poza dopuszczalny zakres ruchu)

            // Lock TargetPosition object while sending data to the robot
            lock (TargetPosition) {
                LastFrameSent = new OutputFrame() {
                    IPOC = LastReceivedFrame.IPOC,
                    Position = TargetPosition
                };

                rsiAdapter.SendData(LastFrameSent);
                TargetPosition.Reset(); //TODO: DLA POZYCJI ABSOLUTNEJ: WYWWALIĆ
            }

            OnFrameSent?.Invoke(LastFrameSent);
        }

        /// <summary>
        /// Stops the robot program execution, raises OnFrameSent event
        /// </summary>
        public void SendError(string errorMessage) {
            LastFrameSent = new OutputFrame() {
                Message = $"Error: {errorMessage}",
                Position = CurrentPosition,
                IPOC = LastReceivedFrame.IPOC
            };

            rsiAdapter.SendData(LastFrameSent);
            OnFrameSent?.Invoke(LastFrameSent);
        }

        /// <summary>
        /// Establish connection with the robot, raises OnInitialize and OnFrameReceived events
        /// </summary>
        public void Initialize() {
            if(isInitialized) {
                return;
            }

            Task.Run(async () => {
                LastReceivedFrame = await rsiAdapter.Connect();
                
                lock (TargetPosition) {
                    //TODO: DLA POZYCJI ABSOLUTNEJ: TargetPosition = (E6POS) CurrentPosition.Clone()
                    TargetPosition = new E6POS();

                    // Send response (prevent connection timeout)
                    rsiAdapter.SendData(new OutputFrame() {
                        IPOC = LastReceivedFrame.IPOC,
                        Position = TargetPosition
                    });
                }

                lock (this) { //TODO: sprawdzic czy lock dla boola ma jakikolwiek sens
                    isInitialized = true;
                }

                OnInitialize?.Invoke();
                OnFrameReceived?.Invoke(LastReceivedFrame);
            });
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Uninitialize() {
            isInitialized = false;
            rsiAdapter.Disconnect();
        }

    }
}
