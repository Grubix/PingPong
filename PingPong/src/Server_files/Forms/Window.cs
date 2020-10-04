using System.Windows.Forms;
using PingPong.Devices;

namespace PingPong {

    public partial class Window : Form {

        private double X, Y, Z, A, B, C = 0.0;

        private readonly Server server;

        private readonly KUKARobot robot1;

        //private readonly KUKARobot robot2;

        //private readonly OptiTrack optiTrack;

        public Window() {
            InitializeComponent();

            robot1 = new KUKARobot("192.168.8.158", 8081);

            server = new Server(robot1);
            server.Start();

            incXBtn.Click += (s, e) => IncreaseValue(ref X);
            decXBtn.Click += (s, e) => DecreaseValue(ref X);

            incYBtn.Click += (s, e) => IncreaseValue(ref Y);
            decYBtn.Click += (s, e) => DecreaseValue(ref Y);

            incZBtn.Click += (s, e) => IncreaseValue(ref Z);
            decZBtn.Click += (s, e) => DecreaseValue(ref Z);

            incABtn.Click += (s, e) => IncreaseValue(ref A);
            decABtn.Click += (s, e) => DecreaseValue(ref A);

            incBBtn.Click += (s, e) => IncreaseValue(ref B);
            decBBtn.Click += (s, e) => DecreaseValue(ref B);

            incCBtn.Click += (s, e) => IncreaseValue(ref C);
            decCBtn.Click += (s, e) => DecreaseValue(ref C);
        }

        public void IncreaseValue(ref double value) {
            value += 1;
            SendData();
        }

        public void DecreaseValue(ref double value) {
            value -= 1;
            SendData();
        }

        public void SendData() {
            robot1.TargetPosition = getTargetPosition();
            server.SendData();
        }

        public E6POS getTargetPosition() {
            return new E6POS(X, Y, Z, A, B, C);
        }
    }

}
