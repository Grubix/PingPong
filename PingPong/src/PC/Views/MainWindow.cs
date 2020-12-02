﻿using MathNet.Numerics.LinearAlgebra;
using PingPong.Applications;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
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
            optiTrack = InitializeOptiTrackSystem();
            ballData = new BallData();
            application = new Ping(robot1, threadSafeChart1);

            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                { -0.009,  0.001, -1.0 },
                { -1.0, -0.002,  0.009 },
                { -0.002,  1.0,  0.001 }
            });

            var translationVector = Vector<double>.Build.DenseOfArray(new double[] {
                791.016, 743.144, 148.319
            });

            /*robot1.Initialized += () => {
                ballData.SetTransformation(robot1, new Transformation(rotationMatrix, translationVector));

                optiTrack.FrameReceived += frame => {
                    ballData.Update(frame);
                    application.ProcessData(ballData);
                };
            };*/

            Transformation transformation = new Transformation(rotationMatrix, translationVector);
            Polyfit2 polyfitX = new Polyfit2(1);
            Polyfit2 polyfitY = new Polyfit2(1);
            Polyfit2 polyfitZ = new Polyfit2(2);

            double Z = 300.0;
            int maxPoints = 100;
            int sampleOffset = 10;

            bool parabolaDrawn = false;
            double tx = 0.0;

            double predX = 0.0;
            double predY = 0.0;
            double predZ = 0.0;
            double timeLeft = 0.0;

            void DrawParabola() {

                parabolaDrawn = true;

                var xCoeffs = polyfitX.CalculateCoefficients();
                var yCoeffs = polyfitY.CalculateCoefficients();
                var zCoeffs = polyfitZ.CalculateCoefficients();
                double[] roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Z);
                double T = 0;
                
                if (roots.Length == 1) {
                    T = roots[0];
                } else if (roots.Length == 2) {
                    T = roots[1];
                }

                predX = xCoeffs[1] * T + xCoeffs[0];
                predY = yCoeffs[1] * T + yCoeffs[0];
                predZ = Z;
                timeLeft = T - tx;

                //Console.WriteLine($"T={timeLeft}, xpred={xCoeffs[1] * T + xCoeffs[0]}, ypred={yCoeffs[1] * T + yCoeffs[0]}");

               /* UpdateUI(() => {
                    for (int i = 0; i < polyfitZ.PointCount; i++) {
                        chart1.Series[0].Points.AddXY(polyfitZ.xValues[i], polyfitZ.yValues[i]);
                    }

                    for (double t = 0; t < T; t += 0.1) {
                        double z = zCoeffs[2] * t * t + zCoeffs[1] * t + zCoeffs[0];
                        chart1.Series[1].Points.AddXY(t, z);
                    }
                });*/
            }

            int samples = 0;

            optiTrack.FrameReceived += frame => {
                var position = transformation.Convert(frame.Position);
                double ballX = position[0];
                double ballY = position[1];
                double ballZ = position[2];

                //TODO: sprawdzic te limity jeszcze raz, czy sie zgadzaja z obecnymi
                if (Z > 250 && ballX != 791.016 && ballY != 743.144 && ballZ != 148.319) {
                    if (polyfitZ.PointCount == maxPoints) {
                        for (int i = 0; i < maxPoints / 2; i++) {
                            polyfitX.xValues[i] = polyfitX.xValues[2 * i];
                            polyfitX.yValues[i] = polyfitX.yValues[2 * i];
                            polyfitY.xValues[i] = polyfitY.xValues[2 * i];
                            polyfitY.yValues[i] = polyfitY.yValues[2 * i];
                            polyfitZ.xValues[i] = polyfitZ.xValues[2 * i];
                            polyfitZ.yValues[i] = polyfitZ.yValues[2 * i];
                        }
                        polyfitX.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                        polyfitX.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                        polyfitY.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                        polyfitY.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                        polyfitZ.xValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                        polyfitZ.yValues.RemoveRange(maxPoints / 2, maxPoints / 2);
                    } else if (samples % sampleOffset == 0) { }
                        //TODO: ogarnąc czas, ewentualnie uzyc stopwatcha
                    
                    polyfitX.AddPoint(tx, ballX);
                    polyfitY.AddPoint(tx, ballY);
                    polyfitZ.AddPoint(tx, ballZ);

                    if (polyfitX.xValues.Count > 30)
                        DrawParabola();

                    (double LowerX, double LowerY, double LowerZ) = robot1.Limits.LowerWorkspaceLimit;
                    (double UpperX, double UpperY, double UpperZ) = robot1.Limits.UpperWorkspaceLimit;

                    if (robot1.Limits.CheckPosition(new E6POS(predX, predY, predZ, robot1.Position.ABC)) && polyfitX.xValues.Count > 30)
                        //robot1.MoveTo(new E6POS(predX, predY, predZ, robot1.Position.ABC), timeLeft);

                    samples++;
                    tx += 0.00416;
                }
                
            };

            /*E6POS current3 = new E6POS(0, 125, 550);
            E6POS current5 = new E6POS(0, 125, 550);
            E6POS target = current3 + new E6POS(150, 0, 0);

            TrajectoryGenerator gen3 = new TrajectoryGenerator(current3);
            TrajectoryGenerator5 gen5 = new TrajectoryGenerator5(current5);

            gen3.SetTargetPosition(target, 4);
            gen5.SetTargetPosition(target, 4);

            Random random = new Random();

            Task.Run(() => {
                while (true) {
                    E6POS c3 = gen3.GetNextCorrection(current3);
                    E6POS c5 = gen5.GetNextCorrection(current5);

                    E6POS rand = new E6POS((random.NextDouble() - 0.5) * 0.01, (random.NextDouble() - 0.5) * 0.01, (random.NextDouble() - 0.5) * 0.01);
                    current3 += c3 + rand;
                    current5 += c5 + rand;

                    threadSafeChart1.AddPoint(current5.X, gen5.Velocity[0]);
                    Thread.Sleep(4);
                }
            });

            incXBtn.Click += (s, e) => {
                target += new E6POS(50, 0, 0);
                gen3.SetTargetPosition(target, 4);
                gen5.SetTargetPosition(target, 4);
            };

            decXBtn.Click += (s, e) => {
                target -= new E6POS(50, 0, 0);
                gen3.SetTargetPosition(target, 4);
                gen5.SetTargetPosition(target, 4);
            };*/
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
                (700, 1000),
                (250, 600),
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

        private void threadSafeChart1_Load(object sender, EventArgs e) {

        }

        private void chart1_Click(object sender, EventArgs e) {

        }
    }
}