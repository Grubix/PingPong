using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PingPong.OptiTrack {
    class CalibrationTool {

        private readonly BackgroundWorker worker;

        /// <summary>
        /// Occurs when calibration is started
        /// </summary>
        public event StartedEventHandler Started;

        /// <summary>
        /// Occurs when calibration progress changed
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Occurs when calibration is completed
        /// </summary>
        public event CompletedEventHandler Completed;

        public delegate void StartedEventHandler();

        public delegate void ProgressChangedEventHandler(int progress);

        public delegate void CompletedEventHandler(Transformation transformation);

        public CalibrationTool() {
            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };
        }

        /// <summary>
        /// Finds <see cref="Transformation">transformation</see> between OptiTrack coordinate system 
        /// and specified KUKA robot coordinate system
        /// </summary>
        /// <param name="optiTrack">optitrack system</param>
        /// <param name="robot">target KUKA robot</param>
        /// <param name="interPoints">number of intermediate points</param>
        /// <param name="optiTrackSamples">optitrack samples per each calibration point</param>
        /// <param name="duration">duration of robot movement (in seconds) between each calibration point</param>
        public void Calibrate(OptiTrackSystem optiTrack, KUKARobot robot, 
            int interPoints = 25, int optiTrackSamples = 200, double duration = 3) {

            if (worker.IsBusy) {
                throw new InvalidOperationException("Calibration in progress");
            }

            if (!optiTrack.IsInitialized()) {
                throw new InvalidOperationException("OptiTrack system is not initialized");
            }

            if (!robot.IsInitialized()) {
                throw new InvalidOperationException("KUKA robot is not initialized");
            }

            E6POS startPoint = robot.LowerWorkspacePoint;
            E6POS endPoint = robot.UpperWorkspacePoint;

            var kukaPoints = new List<Vector<double>>();
            var optiTrackPoints = new List<Vector<double>>();
            var calibrationPoints = GetCalibrationPoints(startPoint, endPoint, interPoints);

            void collectPoints(object sender, DoWorkEventArgs args) {
                // Safe (slow) move to start point
                robot.ForceMoveTo(startPoint, 10);

                for (int i = 0; i < calibrationPoints.Count; i++) {
                    E6POS point = calibrationPoints[i];
                    robot.ForceMoveTo(point, duration);

                    var kukaPoint = point.XYZ;
                    kukaPoints.Add(kukaPoint);

                    var optiTrackPoint = optiTrack.GetAveragePosition(optiTrackSamples);
                    optiTrackPoints.Add(optiTrackPoint);

                    ProgressChanged?.Invoke(i * 100 / (calibrationPoints.Count - 1));
                }
            }
            
            worker.DoWork += collectPoints;
            worker.RunWorkerCompleted += (sender, args) => {
                worker.DoWork -= collectPoints;

                if (args.Error != null) {
                    throw args.Error;
                }

                if (!args.Cancelled) {
                    Completed?.Invoke(new Transformation(optiTrackPoints, kukaPoints));
                }
            };

            worker.RunWorkerAsync();
            Started?.Invoke();
        }

        public void Cancel() {
            worker.CancelAsync();
        }

        private List<E6POS> GetCalibrationPoints(E6POS startPoint, E6POS endPoint, int interPoints) {
            List<E6POS> points = new List<E6POS>();
            int totalPoints = 2 + interPoints;

            E6POS deltaPosition = new E6POS(
                (endPoint.X - startPoint.X) / (interPoints + 1),
                (endPoint.Y - startPoint.Y) / (interPoints + 1),
                (endPoint.Z - startPoint.Z) / (interPoints + 1),
                (endPoint.A - startPoint.A) / (interPoints + 1),
                (endPoint.B - startPoint.B) / (interPoints + 1),
                (endPoint.C - startPoint.C) / (interPoints + 1)
            );

            for (int i = 0; i < totalPoints; i++) {
                points.Add(startPoint + new E6POS(
                    deltaPosition.X * i,
                    deltaPosition.Y * i,
                    deltaPosition.Z * i,
                    deltaPosition.A * i,
                    deltaPosition.B * i,
                    deltaPosition.C * i
                ));
            }

            foreach (var el in points) {
                Console.WriteLine(el);
            }

            return points;
        }

    }
}
