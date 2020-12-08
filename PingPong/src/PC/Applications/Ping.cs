using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 283.81;

        private readonly KUKARobot robot;

        private readonly int maxPoints = 100;

        private readonly double stabilityParameter = 0.005;

        private Polyfit2 polyfitX = new Polyfit2(1);
        
        private Polyfit2 polyfitY = new Polyfit2(1);

        private Polyfit2 polyfitZ = new Polyfit2(2);

        private bool parabolaDrawn = false;

        private bool spat = false;

        private double T;

        private readonly Chart chart;

        private double tx;

        private int sample;

        private List<double> xCoeffs, yCoeffs, zCoeffs, timeOf3Pred;

        public Ping(KUKARobot robot, Chart chart) {
            this.robot = robot;
            this.chart = chart;
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            var position = robot.OptiTrackTransformation.Convert(data.Position);
            double ballX = position[0];
            double ballY = position[1];
            double ballZ = position[2];

            if (ballZ < 0 && ballZ != 148.319) {
                spat = true;

                if (parabolaDrawn) {
                    return;
                }

                parabolaDrawn = true;
                //UpdateUI(() => {
                //    for (int i = 0; i < polyfitZ.PointCount; i++) {
                //        chart.Series[0].Points.AddXY(polyfitZ.xValues[i], polyfitZ.yValues[i]);
                //    }

                //    //for (double t = 0; t < T; t += 0.1) {
                //    //    double z = zCoeffs[2] * t * t + zCoeffs[1] * t + zCoeffs[0];
                //    //    chart.Series[1].Points.AddXY(t, z);
                //    //}
                //});
            }

            if (!spat && ballZ > 250 && ballX < 1300 && ballX != 791.016 && ballY != 743.144 && ballZ != 148.319) {
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

                polyfitX.AddPoint(tx, ballX);
                polyfitY.AddPoint(tx, ballY);
                polyfitZ.AddPoint(tx, ballZ);

                if (polyfitX.xValues.Count > 50) {
                    var prediction = CalculatePrediction();
                    var predPosition = new E6POS(prediction.predX, prediction.predY, Zlevel, robot.Position.ABC);
                    double k = Math.Max(2 * Math.Exp(1 - 1.5 * tx / (prediction.timeLeft + tx)), 1.0);

                    double t = prediction.timeLeft * k;

                    //Console.WriteLine($"t={t}, {predPosition}");
                    //UpdateUI(() => {
                    //    chart.Series[0].Points.AddXY(sample++ , k);
                    //});
                    UpdateUI(() => {
                        chart.Series[0].Points.AddXY(sample++, t);
                    });
                    if (IsTimeStable() && robot.Limits.WorkspaceLimits.CheckPosition(predPosition) && t > 0.0) {
                        robot.MoveTo(predPosition, t);
                    }
                }

                tx += data.FrameDeltaTime;
            }
        }

        (double predX, double predY, double timeLeft) CalculatePrediction() {
            xCoeffs = polyfitX.CalculateCoefficients();
            yCoeffs = polyfitY.CalculateCoefficients();
            zCoeffs = polyfitZ.CalculateCoefficients();
            double[] roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Zlevel);
            T = 0;

            if (roots.Length == 1) {
                T = roots[0];
            } else if (roots.Length == 2) {
                T = roots[1];
            }

            double predX = xCoeffs[1] * T + xCoeffs[0];
            double predY = yCoeffs[1] * T + yCoeffs[0];
            double timeLeft = T - tx;

            AddTimePredToCheckStability(T);

            return (predX, predY, timeLeft);
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
