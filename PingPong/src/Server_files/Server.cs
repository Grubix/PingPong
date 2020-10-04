using PingPong.Devices;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong {
    class Server {

        public KUKARobot Robot1 { get; private set; }

        //public KUKARobot Robot2 { get; private set; }

        //public OptiTrack OptiTrack { get; private set; }

        private bool isRunning = false;

        public Server(KUKARobot robot1) {
            Robot1 = robot1;
            //Robot2 = robot2;
            //OptiTrack = optiTrack;
        }

        public void Start() {
            Robot1.Initialize();
            //Robot2.Initialiize();
            //OptiTrack.Initialize();

            if (!isRunning) {
                isRunning = true;

                Task.Run(async () => {
                    while (isRunning) {
                        await Robot1.ReceiveData();
                        //await Robot2.ReceiveData();
                    }
                });
            }
        }

        public void SendData() {
            Robot1.SendData();
            //Robot2.SendData();
        }

        public void Stop() {
            isRunning = false;
            Robot1.CloseConnection();
            //Robot2.CloseConnection();
        }

        public void PrintAvailableIps() {
            //TODO: do wywalenia / wrzucenia gdzies jako opcja w menu
            IPAddress[] availableIps = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in availableIps) {
                Console.WriteLine(ip);
            }
        }

    }
}
