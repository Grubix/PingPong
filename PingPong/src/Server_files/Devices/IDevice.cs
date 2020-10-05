namespace PingPong.Devices {
    interface IDevice {

        ///<summary>
        ///Initialize device
        ///</summary>
        void Initialize();

        /// <summary>
        /// Check if device is ready to use
        /// </summary>
        /// <returns>true if device is ready to use, false otherwise</returns>
        bool IsInitialized();

    }
}