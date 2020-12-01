using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong.Views {
    public partial class MainWindow : Form {

        private KUKARobot robot1;

        private KUKARobot robot2;

        private OptiTrackSystem optiTrack;

        private BallData ballData;

        private CalibrationWindow calibrationWindow;

        private IApplication application;

        public MainWindow() {
            InitializeComponent();
            InitializeControls();
            robot1 = InitializeRobot1();
            robot2 = InitializeRobot2();
            //optiTrack = InitializeOptiTrackSystem();
            ballData = new BallData();
            application = new Ping(robot1);
            
            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                { -0.011099,  0.0010454, -0.9999370 },
                { -0.999938, -0.0008780,  0.0110987 },
                { -0.000866,  0.9999900,  0.0010551 }
            });

            var translationVector = Vector<double>.Build.DenseOfArray(new double[] {
                817.21905, 613.07449, 143.92211
            });

            //E6POS current3 = new E6POS(0, 125, 550);
            //E6POS current5 = new E6POS(0, 125, 550);
            //E6POS target = current3 + new E6POS(150, 0, 0);

            //TrajectoryGenerator gen3 = new TrajectoryGenerator(current3);
            //TrajectoryGenerator5 gen5 = new TrajectoryGenerator5(current5);

            //gen3.SetTargetPosition(target, 4);
            //gen5.SetTargetPosition(target, 4);

            //Random random = new Random();

            //Task.Run(() => {
            //    while (true) {
            //        E6POS c3 = gen3.GetNextCorrection(current3);
            //        E6POS c5 = gen5.GetNextCorrection(current5);

            //        E6POS rand = new E6POS((random.NextDouble() - 0.5) * 0.01, (random.NextDouble() - 0.5) * 0.01, (random.NextDouble() - 0.5) * 0.01);
            //        current3 += c3 + rand;
            //        current5 += c5 + rand;

            //        threadSafeChart1.AddPoint(current5.X, gen5.Velocity[0]);
            //        Thread.Sleep(4);
            //    }
            //});

            //incXBtn.Click += (s, e) => {
            //    target += new E6POS(50, 0, 0);
            //    gen3.SetTargetPosition(target, 4);
            //    gen5.SetTargetPosition(target, 4);
            //};

            //decXBtn.Click += (s, e) => {
            //    target -= new E6POS(50, 0, 0);
            //    gen3.SetTargetPosition(target, 4);
            //    gen5.SetTargetPosition(target, 4);
            //};
        }

        public void ShowCalibrationWindow() {
            if (calibrationWindow == null || calibrationWindow.IsDisposed) {
                calibrationWindow = new CalibrationWindow(optiTrack, ballData, robot1, robot2);
            }

            calibrationWindow.Show();
            calibrationWindow.Activate();
            calibrationWindow.WindowState = FormWindowState.Normal;
            calibrationWindow.Location = new Point(Location.X, Location.Y);
        }

        private void InitializeControls() {
            incXBtn.Click += (s, e) => robot1.Shift(new E6POS(50, 0, 0), 10.0);
            decXBtn.Click += (s, e) => robot1.Shift(new E6POS(-50, 0, 0), 10.0);

            incYBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 50, 0), 10.0);
            decYBtn.Click += (s, e) => robot1.Shift(new E6POS(0, -50, 0), 10.0);

            incZBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 50), 10.0);
            decZBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, -50), 10.0);

            incABtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 1, 0, 0), 10.0);
            decABtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, -1, 0, 0), 10.0);

            incBBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 1, 0), 10.0);
            decBBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, -1, 0), 10.0);

            incCBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 0, 1), 10.0);
            decCBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 0, -1), 10.0);

            calibrationBtn.Click += (s, e) => ShowCalibrationWindow();
        }

        private KUKARobot InitializeRobot1() {
            RobotLimits limits = new RobotLimits(
                (-450, 450),
                (600, 1000),
                (200, 750),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (1, 0.05)
            );

            KUKARobot robot1 = new KUKARobot(8081, limits);
            robot1Panel.AssignRobot(robot1);
            robot1.Initialize();

            return robot1;
        }

        private KUKARobot InitializeRobot2() {
            //TODO: limity dla drugiego robota!
            RobotLimits limits = null;

            KUKARobot robot2 = new KUKARobot(8082, limits);
            //robot2Panel.SetKUKARobot(robot2);
            robot2.Initialize();

            return robot2;
        }

        private OptiTrackSystem InitializeOptiTrackSystem() {
            OptiTrackSystem optiTrack = new OptiTrackSystem();

            optiTrack.FrameReceived += frame => {
                ballData.Update(frame);
            };

            optiTrack.Initialize();

            return optiTrack;
        }

        private void UpdateUI(Action updateAction) {
            if (InvokeRequired) {
                Action actionWrapper = () => {
                    updateAction.Invoke();
                };

                Invoke(actionWrapper);
                return;
            }

            updateAction.Invoke();
        }

    }
}