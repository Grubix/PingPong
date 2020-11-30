using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            optiTrack = InitializeOptiTrackSystem();
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

            //Transformation transformation = new Transformation(rotationMatrix, translationVector);
            //Polyfit2 polyfitX = new Polyfit2(1);
            //Polyfit2 polyfitY = new Polyfit2(1);
            //Polyfit2 polyfitZ = new Polyfit2(2);

            //double Z = 300.0;
            //int maxPoints = 40;
            //int sampleOffset = 10;

            //bool parabolaDrawn = false;

            //void DrawParabola() {
            //    if (parabolaDrawn) {
            //        return;
            //    }

            //    parabolaDrawn = true;

            //    var xCoeffs = polyfitX.CalculateCoefficients();
            //    var yCoeffs = polyfitY.CalculateCoefficients();
            //    var zCoeffs = polyfitZ.CalculateCoefficients();
            //    double T = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Z)[1];
            //    Console.WriteLine($"T={T}, xpred={xCoeffs[1] * T + xCoeffs[0]}, ypred={yCoeffs[1] * T + yCoeffs[0]}");

            //    UpdateUI(() => {
            //        for (int i = 0; i < polyfitZ.PointCount; i++) {
            //            chart1.Series[0].Points.AddXY(polyfitZ.xValues[i], polyfitZ.yValues[i]);
            //        }

            //        for (double t = 0; t < T; t += 0.1) {
            //            double z = zCoeffs[2] * t * t + zCoeffs[1] * t + zCoeffs[0];
            //            chart1.Series[1].Points.AddXY(t, z);
            //        }
            //    });
            //}

            //int samples = 0;

            //optiTrack.FrameReceived += frame => {
            //    var position = transformation.Convert(frame.Position);
            //    double ballX = position[0];
            //    double ballY = position[1];
            //    double ballZ = position[2];

            //    //TODO: sprawdzic te limity jeszcze raz, czy sie zgadzaja z obecnymi
            //    if (!parabolaDrawn && ballX > -390 && ballX < 500 && ballY > -100 && ballY < 700 && Z > 100) {
            //        if (polyfitZ.PointCount == maxPoints) {
            //            DrawParabola();
            //        } else if (samples % sampleOffset == 0) {
            //            //TODO: ogarnąc czas, ewentualnie uzyc stopwatcha

            //            polyfitX.AddPoint(0, ballX);
            //            polyfitY.AddPoint(0, ballY);
            //            polyfitZ.AddPoint(0, ballZ);

            //            samples++;
            //        }
            //    }
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
                (-300, 300),
                (600, 900),
                (500, 750),
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