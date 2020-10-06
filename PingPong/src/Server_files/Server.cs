using PingPong.Devices;
using PingPong.Modes;
using System.Threading.Tasks;

namespace PingPong {
    class Server {

        public KUKARobot Robot1 { get; private set; }

        //public KUKARobot Robot2 { get; private set; }

        //public OptiTrack OptiTrack { get; private set; }

        private bool isRunning = false;

        public IMode Mode { get; set; }

        public Server(KUKARobot robot1) {
            Robot1 = robot1;
        }

        public void Start() {
            // Initialize all devices
            Robot1.Initialize();

            if (!isRunning) {
                isRunning = true;

                Task.Run(async () => {
                    // Wait until all devices are ready
                    while (true) {
                        if (Robot1.IsInitialized()) {
                            break;
                        }
                    }

                    while (isRunning) {
                        // Update robot data (cartesian position, IPOC etc.)
                        await Robot1.ReceiveDataAsync();

                        // Calculate target position depending on current mode
                        Mode.Compute(Robot1);

                        // Move to target position
                        Robot1.MoveToTargetPosition();
                    }
                });
            }
        }

        public void Stop() {
            isRunning = false;
            Robot1.CloseConnection();
        }

    }
}