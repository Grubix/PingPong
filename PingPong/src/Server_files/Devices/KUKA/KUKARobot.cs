using System;
using System.Threading.Tasks;

namespace PingPong.Devices {
    class KUKARobot : IDevice {

        private readonly RSIAdapter rsiAdapter;

        //TODO: dla set może być ustawione sprawdzenie czy ruch będzie w bezpieczbym zakresie
        public E6POS CurrentPosition { get; private set; }

        public E6POS TargetPosition { get; set; }

        public KUKARobot(string ip, int port) {
            rsiAdapter = new RSIAdapter(ip, port);
        }

        public async Task<InputFrame> ReceiveData() {
            InputFrame data = await rsiAdapter.ReceiveData();

            //TODO: Odebranie danych z kuki i zupdatowanie wszytkich pól (np Position)

            return data;
        }

        public void SendData() {
            if(TargetPosition == CurrentPosition || TargetPosition.Equals(CurrentPosition)) {
                //TODO: to nie koniecznie bedzie dzialac, wszystko zalezy od zaokraglen na Kuce i kompie
                return;
            }

            rsiAdapter.SendData(new OutputFrame(TargetPosition));
        }

        public void CloseConnection() {
            rsiAdapter.CloseConnection();
        }

        public void Initialize() {
            CurrentPosition = new E6POS();
            TargetPosition = CurrentPosition;
            Console.WriteLine("Odebranie aktualnej pozycji robota i wszystkich innych potrzbenych informacji");
            //TODO: Odebranie aktualnej pozycji robota i wszystkich innych potrzbenych informacji
        }

    }
}
