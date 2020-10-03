using System;
using System.Threading.Tasks;

namespace PingPong.Devices {
    class KUKARobot : IDevice {

        private readonly RSIAdapter rsiAdapter;

        public E6POS Position { get; set; } //TODO: dla set może być ustawione sprawdzenie czy ruch będzie w bezpieczbym zakresie

        public KUKARobot(string ip, int port) {
            rsiAdapter = new RSIAdapter(ip, port);
        }

        public async Task<InputFrame> ReceiveData() {
            InputFrame data = await rsiAdapter.ReceiveData();

            //TODO: Odebranie danych z kuki i zupdatowanie wszytkich pól (np Position)

            return data;
        }

        public void SendData() {
            rsiAdapter.SendData(new OutputFrame(Position));
        }

        public void CloseConnection() {
            rsiAdapter.CloseConnection();
        }

        public void Initialize() {
            Position = new E6POS();
            Console.WriteLine("Odebranie aktualnej pozycji robota i wszystkich innych potrzbenych informacji");
            //TODO: Odebranie aktualnej pozycji robota i wszystkich innych potrzbenych informacji
        }

    }
}
