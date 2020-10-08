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

        public event FrameReceivedEventHandler OnFrameReceived;

        public OptiTrack(ConnetionType connetionType = ConnetionType.Multicast) {
            natNetClient = new NatNetClientML((int) connetionType);
        }

        public void Calibrate(KUKARobot robot1) { //TODO: docelowo dwa roboty
            if (!robot1.IsInitialized()) {
                throw new Exception("KUKA robots must be initialized before optitrack");
            }

            //TODO: kalibracja z wykorzystaniem ZAINICJALIZOWANYCH ROBOTÓW !!
        }

        public void Initialize() {
            if(isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if(status != 0) {
                throw new Exception("Optitrack initialization failed");
            }

            // TEST TEST TEST
            natNetClient.OnFrameReady += (data, client) => {
                Console.WriteLine(data);
                //TODO: OnFrameReceived?.Invoke(); Przekazanie jako argument jakichs sensownych danych
            };
            // TEST TEST TEST

            isInitialized = true;
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Disconnect() {
            natNetClient.Uninitialize();
        }

        public delegate void FrameReceivedEventHandler(); //TODO: co przekazywać jako argument ??

    }
}
