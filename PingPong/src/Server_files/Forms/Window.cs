using PingPong.Modes;
using PingPong.Devices;
using System.Windows.Forms;

using Resilio_Project;

namespace PingPong {

    public partial class Window : Form {

        private double X, Y, Z, A, B, C = 0.0;

        private readonly ManualMode mode;

        private readonly Server server;

        private readonly KUKARobot robot1;

        //private readonly KUKARobot robot2;

        //private readonly OptiTrack optiTrack;

        public Window() {
            InitializeComponent();

            /* *** TEST *** */

            RSIServer server = new RSIServer();
            server.StartServer();

            /* *** TEST *** */

            //mode = new ManualMode();
            //robot1 = new KUKARobot(8081);

            //server = new Server(robot1);
            //server.Mode = mode;
            //server.Start();

            //incXBtn.Click += (s, e) => IncreaseValue(ref X);
            //decXBtn.Click += (s, e) => DecreaseValue(ref X);

            //incYBtn.Click += (s, e) => IncreaseValue(ref Y);
            //decYBtn.Click += (s, e) => DecreaseValue(ref Y);

            //incZBtn.Click += (s, e) => IncreaseValue(ref Z);
            //decZBtn.Click += (s, e) => DecreaseValue(ref Z);

            //incABtn.Click += (s, e) => IncreaseValue(ref A);
            //decABtn.Click += (s, e) => DecreaseValue(ref A);

            //incBBtn.Click += (s, e) => IncreaseValue(ref B);
            //decBBtn.Click += (s, e) => DecreaseValue(ref B);

            //incCBtn.Click += (s, e) => IncreaseValue(ref C);
            //decCBtn.Click += (s, e) => DecreaseValue(ref C);
        }

        public void IncreaseValue(ref double value) {
            value += 1;
            mode.TargetPosition = new E6POS(X, Y, Z, A, B, C);
        }

        public void DecreaseValue(ref double value) {
            value -= 1;
            mode.TargetPosition = new E6POS(X, Y, Z, A, B, C);
        }

    }

}
