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
                new double[] { 40.0, -100.0, 350.0 },
                new double[] { 390.0, 250.0, 600.0 },
                /*new double[] { 50.0, -160.0, 635.0 },
                new double[] { 590.0, 300.0, 870.0 },*/
                0.5,
                0.05
            ));

            robot1.FrameReceived += fr => {
                realTimeChart.AddPoint(robot1.trajectoryGenerator.currentVelocity[5]);
            };
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

            incCBtn.Click += (s, e) => robot1.TargetPosition += new E6POS(0, 0, 0, 0, 0, 0.01);
            decCBtn.Click += (s, e) => robot1.TargetPosition -= new E6POS(0, 0, 0, 0, 0, 0.01);

            //Task.Run(() => {
            //    for (int i = 0; i < 10000; i++) {
            //        realTimeChart.AddPoint(Math.Sin(i / 50.0) * Math.Cos(1.2 * Math.Sin(i / 50.0)));
            //        Thread.Sleep(5);
            //    }
            //});
        }

    }

}
