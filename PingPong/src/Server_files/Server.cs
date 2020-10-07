using PingPong.Devices;
using PingPong.Tasks;

namespace PingPong {
    class Server {

        public KUKARobot Robot1 { get; private set; }

        //public KUKARobot Robot2 { get; private set; }

        //public OptiTrack OptiTrack { get; private set; }

        private bool isRunning = false;

        public ITask Task { get; set; }

        public Server(KUKARobot robot1, ITask task) {
            Robot1 = robot1;
            Task = task;
        }

        public void Start() {
            if (!isRunning) {
                // Initialize all devices
                Robot1.Initialize();

                //TODO: Dla drugiego robota stworzyc kolejny wątek. Zastanowić się co wtedy powinno być w interfejsie ITask i jak to ze sobą zsynchronizować

                System.Threading.Tasks.Task.Run(async () => {
                    // Wait until all devices are ready
                    while (true) {
                        if (Robot1.IsInitialized()) {
                            break;
                        }
                    }

                    // Send response to robot (prevent connection timeout)
                    Robot1.MoveToTargetPosition();

                    while (isRunning) {
                        // Update robot data (cartesian position, IPOC etc.)
                        await Robot1.ReceiveDataAsync();

                        // Calculate target position depending on current mode
                        Task.CalculateTargetPosition(Robot1);

                        // Move to target position
                        Robot1.MoveToTargetPosition();
                    }
                });

                isRunning = true;
            }
        }

        public void Stop() {
            isRunning = false;
            Robot1.CloseConnection();
        }

    }
}