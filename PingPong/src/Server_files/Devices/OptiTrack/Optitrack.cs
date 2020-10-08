using System;
using NatNetML;

namespace PingPong.Devices {
    class OptiTrack : IDevice {

        public enum ConnetionType : int {
            Unicast = 0,
            Multicast = 1
        }

        private bool isInitialized = false;

        private readonly NatNetClientML natNetClient;

        public OptiTrack(ConnetionType connetionType) {
            natNetClient = new NatNetClientML((int) connetionType);
        }

        public void Calibrate(KUKARobot robot1) { //TODO: docelowo dwa roboty
            if (!robot1.IsInitialized()) {
                throw new Exception("KUKA robots must be initialized before optitrack");
            }

            //TODO: kalibracja
        }

        public void Initialize() {
            if(isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if(status != 0) {
                throw new Exception("Optitrack initialization failed.");
            }

            // TEST TEST TEST
            natNetClient.OnFrameReady += (data, client) => {
                Console.WriteLine(data);
            };
            // TEST TEST TEST

            //TODO: Pobranie wszystkich informacji, kalibracja itd.
            //TODO: W konstruktorze mozna dac flage czy jest juz jakas kalibracja wgrana (np jakis plik czy cos) zeby
            //TODO: nie trzeba bylo robic tej kalibracji za kazdym razem od nowa

            isInitialized = true;
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Disconnect() {
            natNetClient.Uninitialize();
        }

    }
}
