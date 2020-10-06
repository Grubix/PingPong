namespace PingPong.Devices {
    class OptiTrack : IDevice {

        private bool isInitialized = false;

        public void Initialize() {
            //TODO: Pobranie wszystkich informacji, kalibracja itd.
            //TODO: Po zakonczeniu: isInitialized = true
        }

        public bool IsInitialized() {
            return isInitialized;
        }
    }
}
