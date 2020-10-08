using PingPong.Devices;
using PingPong.Tasks;

namespace PingPong {
    class Server {

        public KUKARobot Robot1 { get; private set; }

        //public KUKARobot Robot2 { get; private set; }

        public OptiTrack OptiTrack { get; private set; }

        private bool isRunning = false;

        public ITask Task { get; set; }

        public Server(KUKARobot robot1, OptiTrack optiTrack, ITask task) {
            Robot1 = robot1;
            OptiTrack = optiTrack;
            Task = task;

            // Set event handlers
            Robot1.OnInitialize += () => StartRobotThread(Robot1);
            // OptiTrack.OnFrameReceived += () => Task.CalculateTargetPosition(Robot1);
            // TODO: do powyższego może być potrzebne użycie locka - modyfikacja Robot1.TargetPosition moze 
            // TODO: nastapic w momencie pobierania danych do ramki (OutputFrame) na innym wątku (StartThreadForRobot)
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start() {
            if (!isRunning) {
                isRunning = true;

                // Initialize devices
                OptiTrack.Initialize();
                Robot1.Initialize();
            }
        }

        private void StartRobotThread(KUKARobot robot) {
            System.Threading.Tasks.Task.Run(async () => {
                while (isRunning) {
                    // Update robot data (cartesian position, IPOC etc.)
                    await robot.ReceiveDataAsync();

                    //TODO: Docelowo Task.CalculateTargetPosition(robot); będzie wywowyłane dopiero po otrzymaniu ramki z optitracka (patrz konstruktor)
                    //TODO: Czyli w tej pętli będzie tylko odbieranie ramki (koniecznie await), a po odebraniu odesłanie danych

                    // Calculate target position depending on current task
                    Task.CalculateTargetPosition(robot);

                    // Move to target position
                    robot.MoveToTargetPosition();
                }
            });
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop() {
            isRunning = false;
            Robot1.Disconnect();
            OptiTrack.Disconnect();
        }

    }
}