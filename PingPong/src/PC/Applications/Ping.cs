using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;
using PingPong.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.Applications {
    class Ping : IApplication {

        private class HitPrediction {

            private const int maxPolyfitPoints = 50;

            private const int timeCheckRange = 5;

            private const double timeErrorTolerance = 0.03;

            private readonly Polyfit polyfitX = new Polyfit(1);

            private readonly Polyfit polyfitY = new Polyfit(1);

            private readonly Polyfit polyfitZ = new Polyfit(2);

            private readonly List<double> predictedTimeSamples = new List<double>();

            public (double X, double Y, double Z) TargetHitPosition { get; set; }

            public double TargetBounceHeight { get; set; }

            public double TimeOfFlight { get; private set; }

            public double TimeToHit { get; private set; }

            public bool IsReady { get; private set; }

            public int PolyfitPointsCount {
                get {
                    return polyfitZ.Values.Count;
                }
            }

            public RobotVector Position {
                get {
                    var xCoeffs = polyfitX.CalculateCoefficients();
                    var yCoeffs = polyfitY.CalculateCoefficients();
                    var zCoeffs = polyfitZ.Coefficients;

                    var ballVelocityOnHit = Vector<double>.Build.DenseOfArray(
                        new double[] { xCoeffs[1], yCoeffs[1], 2.0 * zCoeffs[2] * TimeOfFlight + zCoeffs[1] }
                    );

                    //TODO: kwestia takiego dobrania tego wektora zeby po zderzeniu pilka leciala do TargetHitPOsition
                    var reflectionVector = Vector<double>.Build.DenseOfArray(
                        new double[] { 0.0, 0.0, 1.0 }
                    );

                    var racketNormalVector = reflectionVector.Normalize(1.0) - ballVelocityOnHit.Normalize(1.0);

                    double predX = xCoeffs[1] * TimeOfFlight + xCoeffs[0];
                    double predY = yCoeffs[1] * TimeOfFlight + yCoeffs[0];
                    double predZ = TargetHitPosition.Z;
                    double predB = Math.Atan2(racketNormalVector[0], racketNormalVector[2]) * 180.0 / Math.PI;
                    double predC = -90.0 - Math.Atan2(racketNormalVector[1], racketNormalVector[2]) * 180.0 / Math.PI;

                    return new RobotVector(predX, predY, predZ, 0, predB, predC);
                }
            }

            public RobotVector Velocity { 
                get {
                    return RobotVector.Zero; //TODO: predkosc zderzenia - do dorobienia regulator do ogarniania wysokosci podbijania
                }
            }

            public void AddMeasurement(Vector<double> position, double elapsedTime) {
                if (PolyfitPointsCount == maxPolyfitPoints) {
                    ShiftPolyfitPoints();
                }

                polyfitX.AddPoint(elapsedTime, position[0]);
                polyfitY.AddPoint(elapsedTime, position[1]);
                polyfitZ.AddPoint(elapsedTime, position[2]);

                TimeOfFlight = CalculatePredictedTimeOfFlight();
                IsReady = TimeOfFlight > 0.1 && IsPredictedTimeStable(TimeOfFlight);
                TimeToHit = IsReady ? TimeOfFlight - elapsedTime : -1.0;
            }

            public void Clear() {
                polyfitX.Values.Clear();
                polyfitY.Values.Clear();
                polyfitZ.Values.Clear();

                IsReady = false;
                TimeToHit = -1.0;
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

            private double CalculatePredictedTimeOfFlight() {
                if (polyfitZ.Values.Count < 5) {
                    return -1.0;
                }

                var zCoeffs = polyfitZ.CalculateCoefficients();

                // z(t) = a0 * t^2 + v0 * t + z0
                // z(T) = a0 * T^2 + v0 * T + z0 = TargetHitPosition.Z
                // T = predicted time of flight

                double a0 = zCoeffs[2];
                double v0 = zCoeffs[1];
                double z0 = zCoeffs[0];

                if (a0 >= 0.0) { // negative acceleration expected (-g/2)
                    return -1.0;
                }

                double delta = v0 * v0 - 4.0 * a0 * (z0 - TargetHitPosition.Z);

                if (delta < 0.0) { // no real roots
                    return -1.0;
                } else {
                    return (-v0 - Math.Sqrt(delta)) / (2.0 * a0);
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

        }

        private readonly KUKARobot robot;

        private readonly OptiTrackSystem optiTrack;

        private readonly HitPrediction prediction;

        private readonly ThreadSafeChart chart;

        private bool robotMovedToHitPosition;

        private double elapsedTime;

        public Ping(KUKARobot robot, OptiTrackSystem optiTrack, ThreadSafeChart chart) {
            this.robot = robot;
            this.optiTrack = optiTrack;
            this.chart = chart;

            prediction = new HitPrediction {
                TargetHitPosition = (0, 800, 177),
                TargetBounceHeight = 500.0
            };
        }

        public void Start() {
            if (!robot.IsInitialized() || !optiTrack.IsInitialized()) {
                throw new InvalidOperationException("Robot and optiTrack system must be initialized");
            }

            // waiting for ball to be visible
            Task.Run(() => {
                ManualResetEvent ballSpottedEvent = new ManualResetEvent(false);
                Vector<double> ballPosition = null;
                Vector<double> prevBallPosition = null;
                bool firstFrame = true;

                void checkBallVisiblity(OptiTrack.InputFrame frame) {
                    if (firstFrame) {
                        firstFrame = false;
                        prevBallPosition = frame.Position;
                        return;
                    }

                    ballPosition = robot.OptiTrackTransformation.Convert(frame.Position);

                    bool positionChanged =
                        ballPosition[0] != prevBallPosition[0] ||
                        ballPosition[1] != prevBallPosition[1] ||
                        ballPosition[2] != prevBallPosition[2];

                    if (ballPosition[0] < 1200.0 && positionChanged) {
                        optiTrack.FrameReceived -= checkBallVisiblity;
                        ballSpottedEvent.Set();
                    } else {
                        prevBallPosition = ballPosition;
                    }
                }

                // wait for ballSpottedEvent.Set() signal
                optiTrack.FrameReceived += checkBallVisiblity;
                ballSpottedEvent.WaitOne();

                // start processing optitrack data
                optiTrack.FrameReceived += ProcessOptiTrackData;
            });
        }

        private void ProcessOptiTrackData(OptiTrack.InputFrame frame) {
            var ballPosition = robot.OptiTrackTransformation.Convert(frame.Position);

            // TODO: teraz mozna tu dac jakis warunek z optitracka
            if (robotMovedToHitPosition && robot.IsTargetPositionReached) {
                // reset prediction
                robotMovedToHitPosition = false;
                elapsedTime = 0.0;
                prediction.Clear();

                // slow down the robot
                //TODO: gdzie robot powinien jechac po odbiciu na XYZie zeby to bylo optymalne?
                //TODO: bo dawanie mu swojej obecnej pozycji to tak 2/10
                robot.MoveTo(new RobotVector(robot.Position.XYZ, robot.HomePosition.ABC), RobotVector.Zero, 3);
            }

            // if true -> ball fell
            if (ballPosition[2] < prediction.TargetHitPosition.Z - 50.0) {
                optiTrack.FrameReceived -= ProcessOptiTrackData;
                robot.Uninitialize();
                return;
            }
            
            // prevent increasing elaspedTime when elapsedTime == 0.0
            if (prediction.PolyfitPointsCount != 0) {
                elapsedTime += frame.DeltaTime;
            }

            prediction.AddMeasurement(ballPosition, elapsedTime);

            if (prediction.IsReady && prediction.TimeToHit >= 0.3) {
                RobotVector predictedPosition = prediction.Position;

                if (robot.Limits.CheckPosition(predictedPosition)) {
                    robotMovedToHitPosition = true;
                    robot.MoveTo(predictedPosition, prediction.Velocity, prediction.TimeToHit);
                }
            }
        }

    }
}