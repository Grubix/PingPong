using System;
using NatNetML;
using PingPong.KUKA;

namespace PingPong.OptiTrack {
    class OptiTrackSystem : IDevice {

        public enum ConnetionType : int {
            Multicast = 0,
            Unicast = 1
        }

        private bool isInitialized = false;

        private readonly NatNetClientML natNetClient;

        private readonly ServerDescription serverDescription;

        public event InitializedEventHandler Initialized;

        public event FrameReceivedEventHandler FrameReceived;

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public OptiTrackSystem(ConnetionType connetionType = ConnetionType.Multicast) {
            natNetClient = new NatNetClientML((int) connetionType);
            serverDescription = new ServerDescription();
        }

        public void Initialize() {
            if(isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if(status != 0) {
                throw new Exception("Optitrack initialization failed. Is Motive application running?");
            }

            status = natNetClient.GetServerDescription(serverDescription);

            if(status != 0) {
                throw new Exception("Optitrack connection failed. Is Motive application running?");
            }

            natNetClient.OnFrameReady += (data, client) => {
                FrameReceived?.Invoke(new InputFrame(data));
            };

            isInitialized = true;
            Initialized?.Invoke();
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Uninitialize() {
            isInitialized = false;
            natNetClient.Uninitialize();
        }

        public void Calibrate(KUKARobot robot, E6POS startPosition, E6POS endPosition) {
            if (!isInitialized || !robot.IsInitialized()) {
                throw new Exception("Optitrack and KUKA robot must be initialized");
            }

            //handler odpalający się za kazdym razem jak zostanie otrzymana ramka z optitracka
            void ProcessFrame(InputFrame receivedFrame) {

                //TODO: TUTAJ DZIAŁA PAN BABSONIK, jakas petla albo cos robiaca te wszystkie obliczenia ktore sa w pdfie

                if (true) { //jakiś warunek mowiacy o zakonczeniu kalibracji
                    FrameReceived -= ProcessFrame;
                }
            }

            FrameReceived += ProcessFrame;

            //TODO: gdzie trzymac wyznaczone macierze rotacji i wekt translacji ? w kuce czy w optitracku ?
        }

    }
}
