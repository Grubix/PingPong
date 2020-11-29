using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PingPong.Forms {
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
            application = new Ping(robot1, threadSafeChart1);
            
            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                { -0.011099,  0.0010454, -0.9999370 },
                { -0.999938, -0.0008780,  0.0110987 },
                { -0.000866,  0.9999900,  0.0010551 }
            });

            var translationVector = Vector<double>.Build.DenseOfArray(new double[] {
                817.21905, 613.07449, 143.92211
            });

            robot1.Initialized += () => {
                ballData.SetTransformation(robot1, new Transformation(rotationMatrix, translationVector));

                optiTrack.FrameReceived += frame => {
                    application.ProcessData(ballData);
                };
            };

            Polyfit2 polyfit = new Polyfit2(2);
            Random random = new Random();

            reset.Click += (s, e) => {
                polyfit.Clear();

                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                chart1.Update();

                double g = 9.81;

                double x0 = 0.0;
                double vx0 = 0.2;

                double z0 = 0.6;
                double vz0 = (double) vInput.Value;

                double k = (double) kInput.Value;
                double m = 0.0027;
                double beta = k / m;

                double zf = 1;
                double zp = 1;
                double t = 0.0;

                while (zp > 0.5 || zf > 0.5) {
                    double xf = vx0 / beta * (1 - Math.Exp(-beta * t)) + x0;
                    zf = 1.0 / beta * (vz0 + g / beta) * (1 - Math.Exp(-beta * t)) - g / beta * t + z0;

                    double xp = vx0 * t + x0;
                    zp = -g / 2.0 * t * t + vz0 * t + z0;

                    chart1.Series[0].Points.AddXY(xf, zf * 1000); //zamiana na mm
                    chart1.Series[1].Points.AddXY(xp, zp * 1000); //zamiana na mm

                    t += 0.01;
                }

                Console.WriteLine(t);
            };
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
                (-390, 390),
                (180, 400),
                (390, 650),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (-360, 360),
                (0.5, 0.05)
            );

            KUKARobot robot1 = new KUKARobot(8081, limits);
            robot1Panel.AssignKUKARobot(robot1);
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