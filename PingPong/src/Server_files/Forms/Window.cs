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

            robot1 = new KUKARobot(8081, new RobotLimits(
                new double[] { -1000.0, -1000.0, -1000.0 },
                new double[] { 1000.0, 1000.0, 1000.0 },
                1.0,
                1.0
            ));

            //robot1.FrameReceived += fr => Console.WriteLine($"\nKUKA1::Received: {fr}");
            //robot1.FrameSent += fs => Console.WriteLine($"KUKA1::Sent: {fs}\n");

            robot1.Initialize();

            incXBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(50, 0, 0);
            decXBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(50, 0, 0);

            incYBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 50, 0);
            decYBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 50, 0);

            incZBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 50);
            decZBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 50);

            //incABtn.Click += (s, e) => robot1.TargetPosition.A++;
            //decABtn.Click += (s, e) => robot1.TargetPosition.A--;

            //incBBtn.Click += (s, e) => robot1.TargetPosition.B++;
            //decBBtn.Click += (s, e) => robot1.TargetPosition.B--;

            //incCBtn.Click += (s, e) => robot1.TargetPosition.C++;
            //decCBtn.Click += (s, e) => robot1.TargetPosition.C--;

            Task.Run(() => {
                for (int i = 0; i < 10000; i++) {
                    realTimeChart.AddPoint(Math.Sin(i / 50.0) * Math.Cos(1.2 * Math.Sin(i / 50.0)));
                    Thread.Sleep(5);
                }
            });
        }

    }

}
