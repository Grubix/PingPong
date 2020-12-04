using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using PingPong.OptiTrack;
using System;
using System.Drawing;
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

            //Polyfit2 polyfitX = new Polyfit2(1);
            //Polyfit2 polyfitY = new Polyfit2(1);
            //Polyfit2 polyfitZ = new Polyfit2(2);

            //double Z = 300.0;
            //int maxPoints = 100;
            //int sampleOffset = 10;

            //bool parabolaDrawn = false;
            //double tx = 0.0;

            //double predX = 0.0;
            //double predY = 0.0;
            //double predZ = 0.0;
            //double timeLeft = 0.0;

            //void DrawParabola() {

            //    parabolaDrawn = true;

            //    var xCoeffs = polyfitX.CalculateCoefficients();
            //    var yCoeffs = polyfitY.CalculateCoefficients();
            //    var zCoeffs = polyfitZ.CalculateCoefficients();
            //    double[] roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Z);
            //    double T = 0;

            //    if (roots.Length == 1) {
            //        T = roots[0];
            //    } else if (roots.Length == 2) {
            //        T = roots[1];
            //    }

            //    predX = xCoeffs[1] * T + xCoeffs[0];
            //    predY = yCoeffs[1] * T + yCoeffs[0];
            //    predZ = Z;
            //    timeLeft = T - tx;

            //    //Console.WriteLine($"T={timeLeft}, xpred={xCoeffs[1] * T + xCoeffs[0]}, ypred={yCoeffs[1] * T + yCoeffs[0]}");

            //   /* UpdateUI(() => {
            //        for (int i = 0; i < polyfitZ.PointCount; i++) {
            //            chart1.Series[0].Points.AddXY(polyfitZ.xValues[i], polyfitZ.yValues[i]);
            //        }

            //        for (double t = 0; t < T; t += 0.1) {
            //            double z = zCoeffs[2] * t * t + zCoeffs[1] * t + zCoeffs[0];
            //            chart1.Series[1].Points.AddXY(t, z);
            //        }
            //    });*/
            //}

            //int samples = 0;

            //optiTrack.FrameReceived += frame => {
            //    var position = transformation.Convert(frame.Position);
            //    double ballX = position[0];
            //    double ballY = position[1];
            //    double ballZ = position[2];

            //    //TODO: sprawdzic te limity jeszcze raz, czy sie zgadzaja z obecnymi
            //    if (Z > 250 && ballX != 791.016 && ballY != 743.144 && ballZ != 148.319) {
            //        if (polyfitZ.PointCount == maxPoints) {
            //            for (int i = 0; i < maxPoints / 2; i++) {
            //                polyfitX.xValues[i] = polyfitX.xValues[2 * i];
            //                polyfitX.yValues[i] = polyfitX.yValues[2 * i];
            //                polyfitY.xValues[i] = polyfitY.xValues[2 * i];
            //                polyfitY.yValues[i] = polyfitY.yValues[2 * i];
            //                polyfitZ.xValues[i] = polyfitZ.xValues[2 * i];
            //                polyfitZ.yValues[i] = polyfitZ.yValues[2 * i];
            //            }
            //            polyfitX.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //            polyfitX.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //            polyfitY.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //            polyfitY.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //            polyfitZ.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //            polyfitZ.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
            //        } else if (samples % sampleOffset == 0) { }
            //            //TODO: ogarnąc czas, ewentualnie uzyc stopwatcha

            //        polyfitX.AddPoint(tx, ballX);
            //        polyfitY.AddPoint(tx, ballY);
            //        polyfitZ.AddPoint(tx, ballZ);

            //        if (polyfitX.xValues.Count > 30)
            //            DrawParabola();

            //        (double LowerX, double LowerY, double LowerZ) = robot1.Limits.WorkspaceLimits.LowerLimit;
            //        (double UpperX, double UpperY, double UpperZ) = robot1.Limits.WorkspaceLimits.UpperLimit;

            //        if (robot1.Limits.CheckPosition(new E6POS(predX, predY, predZ, robot1.Position.ABC)) && polyfitX.xValues.Count > 30)
            //            //robot1.MoveTo(new E6POS(predX, predY, predZ, robot1.Position.ABC), timeLeft);

            //        samples++;
            //        tx += 0.00416;
            //    }

            //};
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
            WorkspaceLimits workspaceLimits = new WorkspaceLimits(
                X: (-450, 450),
                Y: (700, 1100),
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

            RobotLimits limits = new RobotLimits(
                workspaceLimits, 
                axisLimits, 
                (2, 0.05)
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

        private void chart1_Click(object sender, EventArgs e) {

        }
    }
}