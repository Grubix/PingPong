using PingPong.KUKA;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class Window : Form {

        private readonly KUKARobot robot1;

        //private readonly KUKARobot robot2;

        //private readonly OptiTrackSystem optiTrack;

        //private ITask task; //TODO: docelowo po odebraniu ramki z optitracka wywołanie metody ComputeTargetPosition()

        public Window() {
            InitializeComponent();

            // Punkty w jakims innym ukladzie
            //new double[] { 50.0, -160.0, 635.0 },
            //new double[] { 590.0, 300.0, 870.0 },

            robot1 = new KUKARobot(8081, new RobotLimits(
                new double[] { 40.0, -100.0, 350.0 },
                new double[] { 390.0, 250.0, 600.0 },
                0.5,
                0.05
            ));

            robot1.FrameReceived += fr => {
                posXText.Text = fr.Position.X.ToString();
                posYText.Text = fr.Position.Y.ToString();
                posZText.Text = fr.Position.Z.ToString();
                posAText.Text = fr.Position.A.ToString();
                posBText.Text = fr.Position.B.ToString();
                posCText.Text = fr.Position.C.ToString();
            };

            robot1.FrameSent += fs => {
                realTimeChart.AddPoint(robot1.CurrentPosition.X, robot1.TargetPosition.X);
            };

            incXBtn.Click += (s, e) => {
                Task.Run(() => {
                    robot1.ForceMoveToPosition(robot1.CurrentPosition + new E6POS(0, 50, 0));
                    robot1.ForceMoveToPosition(robot1.CurrentPosition - new E6POS(0, 0, 50));
                    robot1.ForceMoveToPosition(robot1.CurrentPosition - new E6POS(0, 50, 0));
                    robot1.ForceMoveToPosition(robot1.CurrentPosition + new E6POS(0, 0, 50));
                }).Wait();
            };

            //incXBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(50, 0, 0);
            decXBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(50, 0, 0);

            incYBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 50, 0);
            decYBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 50, 0);

            incZBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 50);
            decZBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 50);

            incABtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 0, 50, 0, 0);
            decABtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 0, 50, 0, 0);

            incBBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 0, 0, 50, 0);
            decBBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 0, 0, 50, 0);

            incCBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 0, 0, 0, 50);
            decCBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 0, 0, 0, 50);

            Task.Run(() => {
                Thread.Sleep(1000);
                for (int i = 0; i < 10000; i++) {
                    double v1 = Math.Abs(Math.Sin(i * 0.004) / (2 + Math.Cos(i * 0.004)));
                    double v2 = Math.Abs(Math.Cos(i * 0.004) / (2 + Math.Sin(i * 0.004)));
                    realTimeChart.AddPoint(Math.Sin(v1 + v2), Math.Cos(v1 + v2) - 0.4);
                    Thread.Sleep(4);
                }
            });

            robot1.Initialize();
        }

    }
}
