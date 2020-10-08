using PingPong.Devices;

namespace PingPong.Tasks {
    class ManualMode : ITask {

        public E6POS TargetPosition { get; set; } = new E6POS();

        public void CalculateTargetPosition(KUKARobot robot1) {
            robot1.TargetPosition = (E6POS) TargetPosition.Clone();
            TargetPosition.Reset(); //TODO: Jeżeli zmienimy na korekcję absolutną koniecznie wywalic!
        }

    }
}