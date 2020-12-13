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
            optiTrack = InitializeOptiTrackSystem();
            application = new Ping(robot1, chart1);

            // Ping
            robot1.Initialized += () => {
                optiTrack.FrameReceived += frame => {
                    application.ProcessData(frame);
                };
            };

            //var clw = new CollisionTest();
            //clw.Show();
            //clw.TopMost = true;

            new CORTester(optiTrack).Show();
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
            WorkspaceLimits workspaceLimits = new WorkspaceLimits(
                X: (-250, 250),
                Y: (700, 950),
                Z: (250, 600)
            );

            AxisLimits axisLimits = new AxisLimits(
                A1: (-360, 360),
                A2: (-360, 360),
                A3: (-360, 360),
                A4: (-360, 360),
                A5: (-360, 360),
                A6: (-360, 360)
            );

            RobotLimits limits = new RobotLimits(workspaceLimits, axisLimits, (2.7, 0.05));
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

            /*optiTrack.FrameReceived += frame => {
                ballData.Update(frame);
            };*/

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
            RobotVector target = new RobotVector(50, 0, 0);
            var gen = new TrajectoryGenerator5v1(current);
            Random rand = new Random();
            gen.SetTargetPosition(current ,target, new RobotVector(150, 0, 0), 3.0);

            Task.Run(() => {
                for (int i = 0; i < 2000; i++) {
                    var corr = gen.GetNextCorrection(current);
                    current += corr;
                    
                    if (!gen.IsTargetPositionReached && corr.X != 0.0 && gen.Velocity.X >= 0.0) {
                        current += new RobotVector(rand.NextDouble() * 0.05, 0, 0);
                    }

                    //threadSafeChart1.AddPoint(current.X, gen.Velocity.X);
                    Thread.Sleep(4);
                }
            });
        }

    }
}