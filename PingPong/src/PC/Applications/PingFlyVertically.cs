using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Views;
using System;
using System.Collections.Generic;

namespace PingPong.Applications {
    class PingFlyVertically : IApplication {

        private const double targetHitHeight = 180.0; // docelowa zetka na ktorej ma nastapic zderzenie

        private const double ballFellHeight = targetHitHeight - 50.0; // wartosc zetki kiedy stwierdziamy ze pileczka spadla (50 mm moze byc za malo)

        private const double timeErrorTolerance = 0.03; // maksymalny blad predykcji czasu miedzy {timeCheckRange} predykcjami ***DO DOPASOWANIA***

        private const int timeCheckRange = 5; // ilosc iteracji przy sprawdzaniu stabilnosci czasu ***DO DOPASOWANIA***

        private const int maxPolyfitPoints = 50; // maksymalna ilosc probek do liczenia polyfita ***DO DOPASOWANIA***

        private readonly ThreadSafeChart chart;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfitX;

        private readonly Polyfit polyfitY;

        private readonly Polyfit polyfitZ;

        private readonly List<double> predictedTimeSamples;

        private Vector<double> prevBallPosition;

        private bool ballSpotted = false; // flaga wyłapująca pierwsze wejscie pilczeki w zakres

        private bool ballFell = false; // jak ta flaga ustawi sie na true, to robot na 100% sie nie ruszy i wlasciwie jest koniec programu

        private bool ballHit = false; // flaga mowiaca o tym ze w teorii nastapilo zderzenie

        private bool robotMoved = false; // flaga mowiaca ze robot wgl sie ruszyl, tzn nastapil podjazd do jakiejs tam predykcji

        private double elapsedTime;

        private Vector<double> reflectionVector = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 1 });

        public PingFlyVertically(KUKARobot robot, ThreadSafeChart chart) {
            this.robot = robot;
            this.chart = chart;

            polyfitX = new Polyfit(1);
            polyfitY = new Polyfit(1);
            polyfitZ = new Polyfit(2);
            predictedTimeSamples = new List<double>();

            // TUTAJ NA RAZIE NA PALE DAC TEN WEKTOR TRANZLACJI Z KUKI
            prevBallPosition = Vector<double>.Build.DenseOfArray(new double[] { 791.016, 743.144, 148.319 });

            robot.FrameReceived += ProcessRobotData;
        }

        public void ProcessRobotData(InputFrame frame) {
            if (ballHit) {
                return;
            }

            if (robotMoved && robot.IsTargetPositionReached) {
                ballHit = true;
                robot.MoveTo(new RobotVector(robot.Position.XYZ, robot.HomePosition.ABC), RobotVector.Zero, 1.5);
            }
        }

        public void ProcessOptiTrackData(OptiTrack.InputFrame data) {
            if (ballFell) {
                return;
            }

            if (ballSpotted) {
                elapsedTime += data.FrameDeltaTime;
            }

            var ballPosition = robot.OptiTrackTransformation.Convert(data.Position);

            if (ballPosition[0] < 1200.0 &&
                (ballPosition[0] != prevBallPosition[0] ||
                ballPosition[1] != prevBallPosition[1] ||
                ballPosition[2] != prevBallPosition[2])
            ) {
                ballSpotted = true;

                if (ballPosition[2] <= ballFellHeight) {
                    ballFell = true;
                    return;
                }

                if (polyfitZ.Values.Count == maxPolyfitPoints) {
                    ShiftPolyfitPoints();
                }

                polyfitX.AddPoint(elapsedTime, ballPosition[0]);
                polyfitY.AddPoint(elapsedTime, ballPosition[1]);
                polyfitZ.AddPoint(elapsedTime, ballPosition[2]);

                var zCoeffs = polyfitZ.CalculateCoefficients();
                double T = CalculatePredictedTimeOfFlight(zCoeffs[2], zCoeffs[1], zCoeffs[0] - targetHitHeight);

                chart.AddPoint(T, T);

                if (!ballHit && T > 0.1 && IsPredictedTimeStable(T)) { // 0.1 mozna zwiekszyc - do przetestowania
                    double timeToHit = T - elapsedTime;

                    if (timeToHit >= 0.05) {
                        var xCoeffs = polyfitX.CalculateCoefficients();
                        var yCoeffs = polyfitY.CalculateCoefficients();

                        var ballTargetVelocity = Vector<double>.Build.DenseOfArray(new double[] { xCoeffs[1], yCoeffs[1], 2 * zCoeffs[2] * T + zCoeffs[1] });
                        Vector<double> paddleNormal = Normalize(reflectionVector) - Normalize(ballTargetVelocity);
                        
                        double angleB = Math.Atan2(paddleNormal[0], paddleNormal[2]) * 180.0 / Math.PI;
                        double angleC = -90.0 - Math.Atan2(paddleNormal[1], paddleNormal[2]) * 180.0 / Math.PI;

                        RobotVector predictedHitPosition = new RobotVector(
                            xCoeffs[1] * T + xCoeffs[0], // predicted x
                            yCoeffs[1] * T + yCoeffs[0], // predicted y
                            targetHitHeight,
                            0,
                            angleB,
                            angleC
                        );

                        Console.WriteLine(predictedHitPosition);

                        if (robot.Limits.WorkspaceLimits.CheckPosition(predictedHitPosition)) {
                            robotMoved = true;
                            RobotVector velocity = new RobotVector(0, 0, 0);
                            //robot.MoveTo(predictedHitPosition, velocity, timeToHit);
                        }
                    }
                }
            }

            prevBallPosition = ballPosition;
        }

        private void ShiftPolyfitPoints() {
            for (int i = 0; i < maxPolyfitPoints / 2; i++) {
                polyfitX.Values[i] = polyfitX.Values[2 * i];
                polyfitY.Values[i] = polyfitY.Values[2 * i];
                polyfitZ.Values[i] = polyfitZ.Values[2 * i];
            }

            polyfitX.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
            polyfitY.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
            polyfitZ.Values.RemoveRange(maxPolyfitPoints / 2, maxPolyfitPoints / 2);
        }

        private double CalculatePredictedTimeOfFlight(double a, double b, double c) {
            if (a >= 0.0) {
                return -1.0;
            }

            double delta = b * b - 4.0 * a * c;

            if (delta < 0) {
                return -1.0;
            } else {
                return (-b - Math.Sqrt(delta)) / (2.0 * a);
            }
        }

        private bool IsPredictedTimeStable(double predictedTime) {
            if (predictedTimeSamples.Count == timeCheckRange) {
                predictedTimeSamples.RemoveAt(0);
                predictedTimeSamples.Add(predictedTime);

                bool isTimeStable = true;

                for (int i = 1; i < predictedTimeSamples.Count; i++) {
                    isTimeStable &= Math.Abs(predictedTimeSamples[i] - predictedTimeSamples[i - 1]) <= timeErrorTolerance;

                    if (!isTimeStable) {
                        return false;
                    }
                }

                return isTimeStable;
            } else {
                predictedTimeSamples.Add(predictedTime);

                return false;
            }
        }

        private Vector<double> Normalize(Vector<double> vec) {
            double vecTvec = 0;
            for (int i = 0; i < vec.Count; i++) {
                vecTvec += vec[i] * vec[i];
            }
            return vec / Math.Sqrt(vecTvec);
        }

    }
}
