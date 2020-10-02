using PingPong.RSI;
using System.Windows.Forms;

namespace PingPong {

    public partial class Window : Form {

        private double X, Y, Z, A, B, C = 0.0;

        private readonly RSIServer server;

        public Window() {
            InitializeComponent();

            server = new RSIServer("127.0.0.1", 8080);
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
            OutputFrame frame = new OutputFrame() {
                X = this.X,
                Y = this.Y,
                Z = this.Z,
                A = this.A,
                B = this.B,
                C = this.C,
            };

            server.Send(frame);
        }
    }

}
