using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 290.46;

        private readonly KUKARobot robot;

        private readonly int maxPoints = 100;

        private readonly double stabilityParameter = 0.005;

        private readonly Polyfit2 polyfitX = new Polyfit2(1);
        
        private readonly Polyfit2 polyfitY = new Polyfit2(1);

        private readonly Polyfit2 polyfitZ = new Polyfit2(2);

        RobotVector predPosition;

        private bool ballFell = false;

        private bool ballHit = false;

        private bool robotMoved = false;

        private RobotVector positionAtHit;

        private readonly Chart chart;

        private double elapsedTime;

        private int sample = 0;

        private List<double> timeOf3Pred = new List<double>(); 

        public Ping(KUKARobot robot, Chart chart) {
            this.robot = robot;
            this.chart = chart;

            robot.FrameReceived += fr => {
                if (robot.IsTargetPositionReached && robotMoved) {
                    ballHit = true;
                    positionAtHit = robot.Position;
                }
            };
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            var position = robot.OptiTrackTransformation.Convert(data.Position);
            double ballX = position[0];
            double ballY = position[1];
            double ballZ = position[2];

            if (ballZ < 0) {
                ballFell = true;
            }

            if (!ballFell && ballZ > 250 && ballX < 1200 && ballX != 791.016 && ballY != 743.144 && ballZ != 148.319) {
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
                }

                polyfitX.AddPoint(elapsedTime, ballX);
                polyfitY.AddPoint(elapsedTime, ballY);
                polyfitZ.AddPoint(elapsedTime, ballZ);

                if (polyfitX.xValues.Count > 40) { //TODO: TUTAJ PAN WOJCIECH M. DORABIA JAKIS FAJNY WARUNEK MOWIACY O TYM ZE CZAS JEST STABILNY
                    var xCoeffs = polyfitX.CalculateCoefficients();
                    var yCoeffs = polyfitY.CalculateCoefficients();
                    var zCoeffs = polyfitZ.CalculateCoefficients();
                    var roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Zlevel);

                    if (roots.Length == 0) {
                        return;
                    }

                    double T = roots[1];
                    double timeLeft = T - elapsedTime;
                    double predX = xCoeffs[1] * T + xCoeffs[0];
                    double predY = yCoeffs[1] * T + yCoeffs[0];

                    UpdateUI(() => {
                        chart.Series[0].Points.AddXY(sample++, timeLeft);
                    });

                    AddTimePredToCheckStability(T);
                    if (sample < 140) {
                        predPosition = new RobotVector(predX, predY, Zlevel, robot.Position.ABC);
                    }

                    //double k = Math.Max(2 * Math.Exp(1 - 1.5 * elapsedTime / (timeLeft + elapsedTime)), 1.0);
                    //Console.WriteLine(predPosition);
                    //Console.WriteLine(timeLeft);
                    //timeLeft *= k;
                    Console.WriteLine(predPosition);

                    if (robot.Limits.WorkspaceLimits.CheckPosition(predPosition) && timeLeft > 0.0) {
                        if (ballHit) {
                            robot.MoveTo(positionAtHit, RobotVector.Zero, 1.5);
                        } else {
                            RobotVector velocity = new RobotVector(0, 0, 20.0, 0, 0, 0);
                            robot.MoveTo(predPosition, velocity, timeLeft);
                        }

                        robotMoved = true;
                    }
                    //Console.WriteLine(predPosition);

                    sample++;
                }

                elapsedTime += data.FrameDeltaTime;
            }
        }

        private void AddTimePredToCheckStability(double time) {
            if (timeOf3Pred.Count >= 3) {
                timeOf3Pred[0] = timeOf3Pred[1];
                timeOf3Pred[1] = timeOf3Pred[2];
                timeOf3Pred[2] = time;
            } else {
                timeOf3Pred.Add(time);
            }
        }

        private bool IsTimeStable() {
            if (timeOf3Pred.Count >= 3) {
                if (Math.Abs(timeOf3Pred[2] - timeOf3Pred[1]) < stabilityParameter
                 && Math.Abs(timeOf3Pred[1] - timeOf3Pred[0]) < stabilityParameter)
                    return true;
            }

            return false;
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
