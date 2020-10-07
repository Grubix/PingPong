using System.Threading.Tasks;

namespace PingPong.Devices {
    class KUKARobot : IDevice {

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

        private E6POS targetPosition;

        public InputFrame LastInputFrame { get; private set; }

        public OutputFrame LastOutputFrame { get; private set; }

        public long IPOC {
            get {
                return LastInputFrame.IPOC;
            }
        }

        public E6POS CurrentPosition {
            get {
                return LastInputFrame.Position;
            }
        }

        public E6POS TargetPosition { 
            get {
                return targetPosition;
            }
            set {
                targetPosition = (E6POS) value.Clone();
            }
        }

        public KUKARobot(int port) {
            rsiAdapter = new RSIAdapter(port);
        }

        /// <summary>
        /// Receives and parses data from the robot
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveDataAsync() {
            LastInputFrame = await rsiAdapter.ReceiveDataAsync();
        }

        public void MoveToTargetPosition() {
            LastOutputFrame = new OutputFrame() {
                IPOC = IPOC,
                Position = TargetPosition
            };

            rsiAdapter.SendData(LastOutputFrame);
        }

        /// <summary>
        /// Sends error and stops the robot program execution
        /// </summary>
        /// <param name="errorMessage">Error message to sent</param>
        public void SendError(string errorMessage) {
            LastOutputFrame = new OutputFrame() {
                Message = $"Error: {errorMessage}",
                Position = CurrentPosition,
                IPOC = IPOC
            };

            rsiAdapter.SendData(LastOutputFrame);
        }

        /// <summary>
        /// Closes connection with the robot
        /// </summary>
        public void CloseConnection() {
            rsiAdapter.CloseConnection();
        }

        public void Initialize() {
            Task.Run(async () => {
                LastInputFrame = await rsiAdapter.Initialize();
                TargetPosition = (E6POS) CurrentPosition.Clone();
                isInitialized = true;
            });
        }

        public bool IsInitialized() {
            return isInitialized;
        }

    }
}
