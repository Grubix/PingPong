using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.LinearAlgebra;

namespace PingPong.Applications {
    class Ping_JuggleVertically : IApplication {

        private const double zPositionAtHit = 340;//290.46;

        private const double timeErrorTolerance = 0.03; // DO SPRAWDZENIA!

        private const int maxPolyfitPoints = 100;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfitX = new Polyfit(1);

        private readonly Polyfit polyfitY = new Polyfit(1);

        private readonly Polyfit polyfitZ = new Polyfit(2);

        private readonly List<double> timeOf3Pred = new List<double>();

        private bool ballFell = false; // jak ta flaga ustawi sie na true, to robot na 100% sie nie ruszy

        private bool robotMoved = false; // flaga mowiaca ze robot wgl sie ruszyl

        private readonly Chart chart;

        private double elapsedTime;

        private int sample;

        private Vector<double> reflectionVector = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 1 });

        public Ping_JuggleVertically(KUKARobot robot, Chart chart) {
            this.robot = robot;
            this.chart = chart;

            robot.FrameReceived += frame => {
                if (robotMoved && robot.IsTargetPositionReached) {
                    var robotPos = new RobotVector(robot.Position.X, robot.Position.Y, 290);
                    robot.MoveTo(robotPos, RobotVector.Zero, 1.5);

                    polyfitX.Values.Clear();
                    polyfitY.Values.Clear();
                    polyfitZ.Values.Clear();
                    timeOf3Pred.Clear();
                }
            };
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            // Pozycja przekonwertowana z układu optitracka do układu odpowiedniej KUKI
            var position = robot.OptiTrackTransformation.Convert(data.Position);
            double ballX = position[0];
            double ballY = position[1];
            double ballZ = position[2];

            if (ballZ < 0) {
                ballFell = true;
            }

            // Zamiast zabawy w te ify trzeba ogarnac LabeledMarkers w optitracku zeby wykryc kiedy dokladnie widzimy pileczke a kiedy nie
            if (!ballFell && ballZ > 250 && ballX < 1200 && ballX != 791.016 && ballY != 743.144 && ballZ != 148.319) {
                if (polyfitZ.Values.Count == maxPolyfitPoints) {
                    for (int i = 0; i < maxPolyfitPoints / 2; i++) {
                        polyfitX.Values[i] = polyfitX.Values[2 * i];
                        polyfitY.Values[i] = polyfitY.Values[2 * i];
                        polyfitZ.Values[i] = polyfitZ.Values[2 * i];
                    }

                    polyfitX.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                    polyfitY.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                    polyfitZ.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                }

                polyfitX.AddPoint(elapsedTime, ballX);
                polyfitY.AddPoint(elapsedTime, ballY);
                polyfitZ.AddPoint(elapsedTime, ballZ);

                var xCoeffs = polyfitX.CalculateCoefficients();
                var yCoeffs = polyfitY.CalculateCoefficients();
                var zCoeffs = polyfitZ.CalculateCoefficients();
                var roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - zPositionAtHit - 20.0);

                if (roots.Length == 0) {
                    // No real roots
                    return;
                }

                double T = roots[1];

                var ballTargetVelocity = Vector<double>.Build.DenseOfArray(new double[] { xCoeffs[1], yCoeffs[1], 2 * zCoeffs[2] * T + zCoeffs[1] });
                Vector<double> paddleNormal = Normalize(reflectionVector) - Normalize(ballTargetVelocity);

                if (IsTimeStable(T) && polyfitX.Values.Count >= 10) {
                    double timeToHit = T - elapsedTime;
                    double predX = xCoeffs[1] * T + xCoeffs[0];
                    double predY = yCoeffs[1] * T + yCoeffs[0];

                    UpdateUI(() => {
                        chart.Series[0].Points.AddXY(sample, predX);
                        chart.Series[1].Points.AddXY(sample++, predY);
                    });
                    Console.WriteLine("T: " + T + " X: " + predX + " Y: " + predY);

                    if (timeToHit >= 0.05) { // 0.1 DO SPRAWDZENIA!
                        RobotVector predictedHitPosition = new RobotVector(
                            predX, predY, zPositionAtHit,
                            0, Math.Atan2(paddleNormal[0], paddleNormal[2]), Math.Atan2(paddleNormal[1], paddleNormal[2])
                         );

                        if (robot.Limits.WorkspaceLimits.CheckPosition(predictedHitPosition)) {
                            //predkosc na osiach w [mm/s]
                            // RobotVector velocity = new RobotVector(0, 0, 0);

                            // Dla odwaznych: 
                            RobotVector velocity = new RobotVector(0, 0, 150);

                            robot.MoveTo(predictedHitPosition, velocity, timeToHit);
                            robotMoved = true;
                        }
                    }
                }

                elapsedTime += data.FrameDeltaTime;
            }
        }

        private bool IsTimeStable(double time) {
            if (timeOf3Pred.Count >= 3) {
                timeOf3Pred[0] = timeOf3Pred[1];
                timeOf3Pred[1] = timeOf3Pred[2];
                timeOf3Pred[2] = time;
            } else {
                timeOf3Pred.Add(time);
            }

            return timeOf3Pred.Count >= 3 &&
                Math.Abs(timeOf3Pred[2] - timeOf3Pred[1]) < timeErrorTolerance &&
                Math.Abs(timeOf3Pred[1] - timeOf3Pred[0]) < timeErrorTolerance;
        }

        private Vector<double> Normalize(Vector<double> vec) {
            double vecTvec = 0;
            for (int i = 0; i < vec.Count; i++) {
                vecTvec += vec[i] * vec[i];
            }
            return vec / Math.Sqrt(vecTvec);
        }

        private void UpdateUI(Action updateAction) {
            if (chart.InvokeRequired) {
                Action actionWrapper = () => {
                    updateAction.Invoke();
                };

                chart.Invoke(actionWrapper);
                return;
            }

            updateAction.Invoke();
        }

    }
}
