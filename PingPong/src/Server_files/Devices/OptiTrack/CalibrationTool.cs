using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace PingPong.OptiTrack {
    class CalibrationTool {

        private readonly OptiTrackSystem optiTrack;

        private readonly BackgroundWorker worker;

        public event StartedEventHandler Started;

        public event ProgressChangedEventHandler ProgressChanged;

        public event CompletedEventHandler Completed;

        public delegate void StartedEventHandler();

        public delegate void ProgressChangedEventHandler(int progress, Transformation transformation);

        public delegate void CompletedEventHandler(Transformation transformation);

        public CalibrationTool(OptiTrackSystem optiTrack) {
            if (!optiTrack.IsInitialized()) {
                throw new InvalidOperationException("OptiTrack system is not initialized");
            }

            this.optiTrack = optiTrack;
            worker = new BackgroundWorker();
        }

        public void Calibrate(KUKARobot robot, uint interPoints, double duration = 5) {
            if (worker.IsBusy) {
                throw new InvalidOperationException("Calibration in progress");
            }

            E6POS startPoint = robot.LowerWorkspacePoint;
            E6POS endPoint = robot.UpperWorkspacePoint;

            List<E6POS> calibrationPoints = GetCalibrationPoints(startPoint, endPoint, interPoints);
            var kukaPoints = new List<Vector<double>>();
            var optiTrackPoints = new List<Vector<double>>();

            int pointsCount = calibrationPoints.Count;
            Transformation transformation = null;

            void collectPoints(object sender, DoWorkEventArgs args) {
                for (int i = 0; i < pointsCount; i++) {
                    E6POS point = calibrationPoints[i];
                    robot.ForceMoveTo(point, duration);

                    var kukaPoint = point.XYZ;
                    kukaPoints.Add(kukaPoint);

                    var optiTrackPoint = optiTrack.GetAveragePosition(200);
                    optiTrackPoints.Add(optiTrackPoint);

                    int progress = i * 100 / (pointsCount - 1);
                    transformation = new Transformation(optiTrackPoints, kukaPoints);

                    ProgressChanged?.Invoke(progress, transformation);
                }
            }

            worker.DoWork += collectPoints;
            worker.RunWorkerCompleted += (sender, args) => {
                worker.DoWork -= collectPoints;

                if (args.Error != null) {
                    throw args.Error;
                }

                Completed?.Invoke(transformation);
            };

            worker.RunWorkerAsync();
            Started?.Invoke();
        }

        private List<E6POS> GetCalibrationPoints(E6POS startPoint, E6POS endPoint, uint interPoints) {
            List<E6POS> points = new List<E6POS>();
            uint totalPoints = 2 + interPoints;

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

            return points;
        }

    }
}
