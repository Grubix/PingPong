using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong.Views {
    public partial class MainWindow : Form {

        private readonly KUKARobot robot1;

        private readonly KUKARobot robot2;

        private readonly OptiTrackSystem optiTrack;

        private CalibrationWindow calibrationWindow;

        private IApplication application;

        public MainWindow() {
            InitializeComponent();
            InitializeControls();
            robot1 = InitializeRobot1();
            robot2 = InitializeRobot2();
            //optiTrack = InitializeOptiTrackSystem();
            application = new Ping(robot1, optiTrack, threadSafeChart);

            PIDRegulator regulator = new PIDRegulator(600, 0, 0, 0.004, 0);
            double u0 = 0, u1 = 0;
            double y0 = 0, y1 = 0;
            double e0 = 0;

            double T = 5;
            Task.Run(() => {
                for (int i = 0; i < 550; i++) {
                    u1 = u0;
                    y1 = y0;

                    double setpoint = -20.0;
                    if (i >= 50) {
                        setpoint = 15.0;
                        if (i >= 100) {
                            setpoint = (i - 99) / 10.0 * Math.Sin(Math.Sin(i / 10.0)) + 15.0;
                            if (i >= 300) {
                                setpoint = 0;
                            }
                        }
                    }

                    (u0, e0) = regulator.Compute(setpoint, y0);
                    y0 = 1.0 / (0.004 + 2.0 * T) * (0.004 * (u0 + u1) - (0.004 - 2.0 * T) * y1);

                    UpdateUI(() => {
                        chart1.Series[0].Points.AddXY(i * 0.004, setpoint);
                        chart1.Series[1].Points.AddXY(i * 0.004, y0);
                    });

                    Thread.Sleep(4);
                }
            });

            // Ping
            robot1.Initialized += () => {
                application.Start();
            };
        }

        public void ShowCalibrationWindow() {
            if (calibrationWindow == null || calibrationWindow.IsDisposed) {
                calibrationWindow = new CalibrationWindow(optiTrack, robot1, robot2);
            }

            calibrationWindow.Show();
            calibrationWindow.Activate();
            calibrationWindow.WindowState = FormWindowState.Normal;
            calibrationWindow.Location = new Point(Location.X, Location.Y);
        }

        private void InitializeControls() {
            incXBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(50, 0, 0), RobotVector.Zero, 5.0);
            decXBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(-50, 0, 0), RobotVector.Zero, 5.0);

            incYBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 50, 0), RobotVector.Zero, 5.0);
            decYBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, -50, 0), RobotVector.Zero, 5.0);

            incZBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 50), RobotVector.Zero, 5.0);
            decZBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, -50), RobotVector.Zero, 5.0);

            incABtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, 1, 0, 0), RobotVector.Zero, 5.0);
            decABtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, -1, 0, 0), RobotVector.Zero, 5.0);

            incBBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, 0, 1, 0), RobotVector.Zero, 5.0);
            decBBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, 0, -1, 0), RobotVector.Zero, 5.0);

            incCBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, 0, 0, 1), RobotVector.Zero, 5.0);
            decCBtn.Click += (s, e) => robot1.MoveTo(robot1.TargetPosition + new RobotVector(0, 0, 0, 0, 0, -1), RobotVector.Zero, 5.0);

            calibrationBtn.Click += (s, e) => ShowCalibrationWindow();
        }

        private KUKARobot InitializeRobot1() {
            RobotLimits limits = new RobotLimits(
                lowerWorkspaceLimit: (-250.0, 700.0, 150.0),
                upperWorkspaceLimit: (250.0, 950.0, 400.0),
                a1AxisLimit: (-360, 360),
                a2AxisLimit: (-360, 360),
                a3AxisLimit: (-360, 360),
                a4AxisLimit: (-360, 360),
                a5AxisLimit: (-360, 360),
                a6AxisLimit: (-360, 360),
                correctionLimit: (2.0, 0.1)
            );

            KUKARobot robot1 = new KUKARobot(8081, limits);

            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                { -0.009,  0.001, -1.0 },
                { -1.0, -0.002,  0.009 },
                { -0.002,  1.0,  0.001 }
            });

            var translationVector = Vector<double>.Build.DenseOfArray(new double[] {
                791.016, 743.144, 148.319
            });

            robot1Panel.AssignRobot(robot1);
            robot1.OptiTrackTransformation = new Transformation(rotationMatrix, translationVector);
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

        private void Test() {
            RobotVector current = new RobotVector();
            RobotVector target = new RobotVector(0, 0, 0);
            var gen = new TrajectoryGenerator5v2(current);
            Random rand = new Random();
            gen.SetTargetPosition(current, target, new RobotVector(150, 0, 0), 3.0);

            Task.Run(() => {
                for (int i = 0; i < 2000; i++) {
                    current = gen.GetNextAbsoluteCorrection(current);
                    
                    if (!gen.IsTargetPositionReached) {
                        //current += new RobotVector(rand.NextDouble() * 0.05, 0, 0);
                    }

                    //threadSafeChart1.AddPoint(current.X, gen.Velocity.X);
                    Thread.Sleep(4);
                }
            });
        }

    }
}