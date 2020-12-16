using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using PingPong.Views;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double zPositionAtHit = 340;//290.46;

        private const double timeErrorTolerance = 0.03; // DO SPRAWDZENIA!

        private const int maxPolyfitPoints = 100;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfitX = new Polyfit(1);

        private readonly Polyfit polyfitY = new Polyfit(1);

        private readonly Polyfit polyfitZ = new Polyfit(2);

        private readonly List<double> timeOf3Pred = new List<double>();

        private bool ballFell = false; // jak ta flaga ustawi sie na true, to robot na 100% sie nie ruszy

        private bool ballHit = false; // flaga mowiaca o tym ze w teorii nastapilo zderzenie

        private bool robotMoved = false; // flaga mowiaca ze robot wgl sie ruszyl

        private readonly ThreadSafeChart chart;

        private double elapsedTime;

        private int sample;

        private RobotVector prevPosition = new RobotVector(791.016, 743.144, 148.319);

        public Ping(KUKARobot robot, ThreadSafeChart chart) {
            this.robot = robot;
            this.chart = chart;

            robot.FrameReceived += frame => {
                if (ballHit) {
                    return;
                }

                if (robotMoved && robot.IsTargetPositionReached) {
                    ballHit = true;
                    robot.MoveTo(robot.Position, RobotVector.Zero, 1.5);
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
            if (!ballFell && ballZ > 0 && ballX < 1200 && prevPosition.X != ballX && prevPosition.Y != ballY && prevPosition.Z != ballZ) {
                /*if (polyfitZ.Values.Count == maxPolyfitPoints) {
                    for (int i = 0; i < maxPolyfitPoints / 2; i++) {
                        polyfitX.Values[i] = polyfitX.Values[2 * i];
                        polyfitY.Values[i] = polyfitY.Values[2 * i];
                        polyfitZ.Values[i] = polyfitZ.Values[2 * i];
                    }

                    polyfitX.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                    polyfitY.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                    polyfitZ.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
                }*/

                prevPosition = new RobotVector(ballX, ballY, ballZ);
                Console.WriteLine(ballX);
                Console.WriteLine(ballY);
                Console.WriteLine(ballZ);

                polyfitX.AddPoint(elapsedTime, ballX);
                polyfitY.AddPoint(elapsedTime, ballY);
                polyfitZ.AddPoint(elapsedTime, ballZ);

                var xCoeffs = polyfitX.CalculateCoefficients();
                var yCoeffs = polyfitY.CalculateCoefficients();
                var zCoeffs = polyfitZ.CalculateCoefficients();
                var roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - zPositionAtHit);

                if (roots.Length == 0) {
                    // No real roots
                    return;
                }

                double T = roots[1];
                if (T < 3)
                    chart.AddPoint(T, 0);

                if (IsTimeStable(T) && polyfitX.Values.Count >= 10) {
                    double timeToHit = T - elapsedTime;
                    double predX = xCoeffs[1] * T + xCoeffs[0];
                    double predY = yCoeffs[1] * T + yCoeffs[0];

                   
                    Console.WriteLine("T: " + T + " X: " + predX + " Y: " + predY);

                    /*if (!ballHit && timeToHit >= 0.05) { // 0.1 DO SPRAWDZENIA!
                        RobotVector predictedHitPosition = new RobotVector(predX, predY, zPositionAtHit, robot.Position.ABC);

                        if (robot.Limits.WorkspaceLimits.CheckPosition(predictedHitPosition)) {
                            //predkosc na osiach w [mm/s]
                            // RobotVector velocity = new RobotVector(0, 0, 0);

                            // Dla odwaznych: 
                            RobotVector velocity = new RobotVector(0, 0, 150);

                            //robot.MoveTo(predictedHitPosition, velocity, timeToHit);
                            robotMoved = true;
                        }
                    }*/
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

    }
}
