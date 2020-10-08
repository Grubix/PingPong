using PingPong.Devices;
using PingPong.Tasks;
using System;

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
        }

        public void Start() {
            if (!isRunning) {
                // Set event handlers
                Robot1.OnInitialize += () => StartThreadForRobot(Robot1);
                //Robot2.OnInitialize += () => StartThreadForRobot(Robot2);

                // Initialize all devices
                OptiTrack.Initialize();
                Robot1.Initialize();
                //Robot2.Initialize();

                isRunning = true;
            }
        }

        private void StartThreadForRobot(KUKARobot robot) {
            System.Threading.Tasks.Task.Run(async () => {
                // Send response to robot (prevent connection timeout)
                robot.MoveToTargetPosition();

                //TODO: Jezeli eventy beda dzialaly jak trzeba mozna wywalic petle

                while (isRunning) {
                    // Update robot data (cartesian position, IPOC etc.)
                    await robot.ReceiveDataAsync();

                    //TODO: Dla drugiego robota stworzyc kolejny wątek. Zastanowić się co wtedy powinno być w interfejsie 
                    //TODO: ITask i jak to ze sobą zsynchronizować

                    // Calculate target position depending on current task
                    Task.CalculateTargetPosition(robot);

                    // Move to target position
                    robot.MoveToTargetPosition();
                }
            });
        }

        public void Stop() {
            isRunning = false;
            Robot1.Disconnect();
            OptiTrack.Disconnect();
        }

        //TODO: callback / delegat / event handler jak zwał tak zwał wywyoływany po zakończeniu pętli (zupdatowanie w okienku info o pozycji, wykresy itd.)

    }
}