namespace PingPong.Devices {
    interface IDevice {

        ///<summary>
        ///Initializes device
        ///</summary>
        void Initialize();

        /// <summary>
        /// Checks if device is ready to use
        /// </summary>
        /// <returns>true if device is ready to use, false otherwise</returns>
        bool IsInitialized();

        /// <summary>
        /// Close connection with the device
        /// </summary>
        void Disconnect();

    }
}