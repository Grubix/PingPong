using System;
using System.Threading.Tasks;

namespace PingPong.Devices {
    class KUKARobot : IDevice {

        private bool isInitialized = false;

        private readonly RSIAdapter rsiAdapter;

        public long IPOC { get; private set; }

        public E6POS CurrentPosition { get; private set; }

        private E6POS _targetPosition;

        public E6POS TargetPosition { 
            get => _targetPosition;
            set => _targetPosition = (E6POS) value.Clone();
        }

        public KUKARobot(int port) {
            rsiAdapter = new RSIAdapter(port);
        }

        public void MoveToTargetPosition() {
            rsiAdapter.SendData(new OutputFrame() {
                IPOC = IPOC,
                Position = TargetPosition
            });
        }

        public async Task ReceiveDataAsync() {
            InputFrame frame = await rsiAdapter.ReceiveDataAsync();
            IPOC = frame.IPOC;
            CurrentPosition = frame.Position;
        }

        public void CloseConnection() {
            rsiAdapter.CloseConnection();
        }

        public void Initialize() {
            Task.Run(async () => {
                InputFrame firstFrame = await rsiAdapter.Initialize();
                IPOC = firstFrame.IPOC;
                CurrentPosition = firstFrame.Position;
                TargetPosition = (E6POS) CurrentPosition.Clone();
                isInitialized = true;
            });
        }

        public bool IsInitialized() {
            return isInitialized;
        }

    }
}
