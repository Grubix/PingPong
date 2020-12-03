using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 300;

        private readonly KUKARobot robot;

        private readonly int maxPoints = 100;

        private Polyfit2 polyfitX = new Polyfit2(1);
        
        private Polyfit2 polyfitY = new Polyfit2(1);

        private Polyfit2 polyfitZ = new Polyfit2(2);

        private readonly Chart chart;

        private double tx;

        public Ping(KUKARobot robot, Chart chart) {
            this.robot = robot;
            this.chart = chart;
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            var position = data.Position;

            double ballX = position[0];
            double ballY = position[1];
            double ballZ = position[2];

            if (ballZ > 250 && ballX != 0.0 && ballY != 0.0 && ballZ != 0.0) {
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

                if (polyfitX.xValues.Count > 5) {
                    var prediction = CalculatePrediction();
                    var predPosition = new E6POS(prediction.predX, prediction.predY, Zlevel, robot.Position.ABC);

                    if (robot.Limits.WorkspaceLimits.CheckPosition(predPosition)) {
                        robot.MoveTo(predPosition, prediction.timeLeft * 1);
                    }
                }

                tx += data.FrameDeltaTime;
            }
        }

        (double predX, double predY, double timeLeft) CalculatePrediction() {
            var xCoeffs = polyfitX.CalculateCoefficients();
            var yCoeffs = polyfitY.CalculateCoefficients();
            var zCoeffs = polyfitZ.CalculateCoefficients();
            double[] roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - Zlevel);
            double T = 0;

            if (roots.Length == 1) {
                T = roots[0];
            } else if (roots.Length == 2) {
                T = roots[1];
            }

            double predX = xCoeffs[1] * T + xCoeffs[0];
            double predY = yCoeffs[1] * T + yCoeffs[0];
            double timeLeft = T - tx;

            Console.WriteLine($"T={timeLeft}, xpred={xCoeffs[1] * T + xCoeffs[0]}, ypred={yCoeffs[1] * T + yCoeffs[0]}");
            return (predX, predY, timeLeft);
        }

    }
}
