using System;
using NatNetML;
using PingPong.Devices.KUKA;

namespace PingPong.Devices.OptiTrack {
    class OptiTrackSystem : IDevice {

        public enum ConnetionType : int {
            Multicast = 0,
            Unicast = 1
        }

        private bool isInitialized = false;

        private readonly NatNetClientML natNetClient;

        public event InitializeEventHandler OnInitialize;

        public event FrameReceivedEventHandler OnFrameReceived;

        public delegate void InitializeEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public OptiTrackSystem(ConnetionType connetionType = ConnetionType.Multicast) {
            natNetClient = new NatNetClientML((int) connetionType);
        }

        public void Calibrate(KUKARobot robot) {
            if (!isInitialized || !robot.IsInitialized()) {
                throw new Exception("Optitrack and KUKA robot must be initialized");
            }

            //TODO: kalibracja z wykorzystaniem ZAINICJALIZOWANEGO robota
        }

        public void Initialize() {
            if(isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if(status != 0) {
                throw new Exception("Optitrack initialization failed. Is Motive application running?");
            }

            natNetClient.OnFrameReady += (data, client) => {
                OnFrameReceived?.Invoke(new InputFrame()); //TODO: Co jest potrzebne w ramce z optitracka ??
            };

            isInitialized = true;
            OnInitialize?.Invoke();
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Uninitialize() {
            isInitialized = false;
            natNetClient.Uninitialize();
        }

    }
}
