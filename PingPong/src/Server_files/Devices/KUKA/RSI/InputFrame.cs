using System;

namespace PingPong.Devices {
    ///<summary>
    ///Frame received from the KUKA robot
    ///</summary>
    public class InputFrame {

        public E6POS Position { get; private set; }

        //TODO: parsed data (np. pozycja (DEF_RIst))

        public InputFrame(string data) {
            //TODO: parse data
            Console.WriteLine($"Received: {data}");
        }

    }
}