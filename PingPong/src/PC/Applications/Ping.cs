using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.Maths.Solver;
using PingPong.Views;
using System;
using System.Collections.Generic;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double ballRadius = 20.0;

        private const double targetHitHeight = 180.0;

        private const double ballFellHeightTreshold = 0.0;

        private const double timeErrorTolerance = 0.03;

        private const int timeCheckRange = 5;

        private const int maxPolyfitPoints = 100;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfitX = new Polyfit(1);

        private readonly Polyfit polyfitY = new Polyfit(1);

        private readonly Polyfit polyfitZ = new Polyfit(2);

        private readonly List<double> predictedTimeSamples = new List<double>();

        private readonly ThreadSafeChart chart;

        private bool ballFell = false; // jak ta flaga ustawi sie na true, to robot na 100% sie nie ruszy

        private bool ballHit = false; // flaga mowiaca o tym ze w teorii nastapilo zderzenie

        private bool robotMoved = false; // flaga mowiaca ze robot wgl sie ruszyl

        private double elapsedTime;

        public Ping(KUKARobot robot, ThreadSafeChart chart) {
            this.robot = robot;
            this.chart = chart;

            robot.FrameReceived += frame => {
                if (ballHit) {
                    return;
                }

                if (robotMoved && robot.IsTargetPositionReached) {
                    ballHit = true;
                    robot.MoveTo(new RobotVector(robot.Position.XYZ, robot.HomePosition.ABC), RobotVector.Zero, 1.5);
                }
            };
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            var ballPosition = robot.OptiTrackTransformation.Convert(data.Position);

            if (ballPosition[2] < ballFellHeightTreshold) {
                ballFell = true;
            }

            if (IsBallVisible(ballPosition)) {
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

                polyfitX.AddPoint(elapsedTime, ballPosition[0]);
                polyfitY.AddPoint(elapsedTime, ballPosition[1]);
                polyfitZ.AddPoint(elapsedTime, ballPosition[2]);

                var xCoeffs = polyfitX.CalculateCoefficients();
                var yCoeffs = polyfitY.CalculateCoefficients();
                var zCoeffs = polyfitZ.CalculateCoefficients();

                // center of the ball on z axis (t) = a0 / 2 * t^2 + v0 * t + z0
                // bottom of the ball on z axis (t) = a0 / 2 * t^2 + v0 * t + z0 - {ball radius}
                // a0 / 2 * t^2 + v0 * t + z0 - {ball radius} = {target hit height}
                // a0 / 2 * t^2 + v0 * t + z0 - {target hit height} - {ball radius} = 0

                var roots = QuadraticSolver.SolveReal(zCoeffs[2], zCoeffs[1], zCoeffs[0] - targetHitHeight - ballRadius);

                if (roots.Length == 0) {
                    return; // No real roots
                }

                // bottom of the ball on z axis(T) = {target hit height} + {ball radius} - z0
                double T = roots[1];

                if (IsTimeStable(T) && polyfitX.Values.Count >= 10) {
                    double timeToHit = T - elapsedTime;
                    double predX = xCoeffs[1] * T + xCoeffs[0];
                    double predY = yCoeffs[1] * T + yCoeffs[0];

                    if (!ballHit && timeToHit >= 0.05) {
                        RobotVector predictedHitPosition = new RobotVector(predX, predY, targetHitHeight, robot.HomePosition.ABC);

                        if (robot.Limits.WorkspaceLimits.CheckPosition(predictedHitPosition)) {
                            robotMoved = true;
                            RobotVector velocity = new RobotVector(0, 0, 0);
                            robot.MoveTo(predictedHitPosition, velocity, timeToHit);
                        }
                    }
                }

                elapsedTime += data.FrameDeltaTime;
            }
        }

        private bool IsBallVisible(Vector<double> ballPosition) {
            var translationVector = robot.OptiTrackTransformation.Translation;

            return
                !ballFell &&
                ballPosition[0] < 1200.0 &&
                ballPosition[2] > 250.0 &&
                ballPosition[0] != translationVector[0] &&
                ballPosition[1] != translationVector[1] &&
                ballPosition[2] != translationVector[2];
        }

        private bool IsTimeStable(double time) {
            if (predictedTimeSamples.Count == timeCheckRange) {
                predictedTimeSamples.RemoveAt(0);
                predictedTimeSamples.Add(time);

                bool isTimeStable = true;

                for (int i = 1; i < predictedTimeSamples.Count; i++) {
                    isTimeStable &= Math.Abs(predictedTimeSamples[i] - predictedTimeSamples[i - 1]) <= timeErrorTolerance;
                }

                return isTimeStable;
            } else {
                predictedTimeSamples.Add(time);

                return false;
            }
        }

    }
}
