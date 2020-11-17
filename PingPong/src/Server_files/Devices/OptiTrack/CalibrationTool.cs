using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PingPong.OptiTrack {
    class CalibrationTool {

        private readonly OptiTrackSystem optiTrack;

        private readonly BackgroundWorker worker;

        private readonly List<Vector<double>> optiTrackPoints;

        private readonly List<Vector<double>> kukaRobotPoints;

        private readonly List<Vector<double>> calibrationPoints;

        private KUKARobot robot;

        private int samplesPerPoint;

        public event Action Start;

        public event Action<int, Transformation> ProgressChanged;

        public event Action<Transformation> Completed;

        public CalibrationTool(OptiTrackSystem optiTrack) {
            this.optiTrack = optiTrack;

            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            optiTrackPoints = new List<Vector<double>>();
            kukaRobotPoints = new List<Vector<double>>();
            calibrationPoints = new List<Vector<double>>();

            Transformation transformation = null;

            worker.DoWork += (s, e) => {
                Start?.Invoke();
                robot.ForceMoveTo(new E6POS(calibrationPoints[0], robot.CurrentPosition.ABC), 10);

                for (int i = 0; i < calibrationPoints.Count; i++) {
                    robot.ForceMoveTo(new E6POS(calibrationPoints[i], robot.CurrentPosition.ABC), 3);

                    var kukaPoint = robot.CurrentPosition.XYZ;
                    kukaRobotPoints.Add(kukaPoint);

                    var optiTrackPoint = optiTrack.GetAveragePosition(samplesPerPoint);
                    optiTrackPoints.Add(optiTrackPoint);

                    int progress = i * 100 / (calibrationPoints.Count - 1);
                    transformation = new Transformation(optiTrackPoints, kukaRobotPoints);

                    ProgressChanged?.Invoke(progress, transformation);
                }

                robot.ForceMoveTo(robot.HomePosition, 10);
            };

            worker.RunWorkerCompleted += (s, e) => {
                if (e.Error != null) {
                    throw e.Error;
                } else {
                    Completed?.Invoke(transformation);
                }
            };
        }

        public void Calibrate(KUKARobot robot, int pointsPerLine = 10, int samplesPerPoint = 200) {
            if (worker.IsBusy) {
                throw new InvalidOperationException("Calibration in progress");
            }

            if (!optiTrack.IsInitialized()) {
                throw new InvalidOperationException("OptiTrack system is not initialized");
            }

            if (!robot.IsInitialized()) {
                throw new InvalidOperationException("KUKA robot is not initialized");
            }

            this.robot = robot;
            this.samplesPerPoint = samplesPerPoint;
            CalculateCalibrationPoints(robot, pointsPerLine);

            worker.RunWorkerAsync();
        }

        public void Cancel() {
            worker.CancelAsync();
        }

        private void CalculateCalibrationPoints(KUKARobot robot, int pointsPerLine) {
            calibrationPoints.Clear();

            var x0 = robot.LowerWorkspacePoint;
            var x1 = robot.UpperWorkspacePoint;

            // Shrink workspace by 5mm
            var offset = Vector<double>.Build.DenseOfArray(new double[] {
                x1[0] > x0[0] ? 5.0 : -5.0,
                x1[1] > x0[1] ? 5.0 : -5.0,
                x1[2] > x0[2] ? 5.0 : -5.0
            });

            x0 += offset;
            x1 -= offset;

            // Cubid points
            var p0 = Vector<double>.Build.DenseOfArray(new double[] { x0[0], x0[1], x0[2] });
            var p1 = Vector<double>.Build.DenseOfArray(new double[] { x1[0], x0[1], x0[2] });
            var p2 = Vector<double>.Build.DenseOfArray(new double[] { x0[0], x1[1], x0[2] });
            var p3 = Vector<double>.Build.DenseOfArray(new double[] { x1[0], x1[1], x0[2] });
            var p4 = Vector<double>.Build.DenseOfArray(new double[] { x0[0], x0[1], x1[2] });
            var p5 = Vector<double>.Build.DenseOfArray(new double[] { x1[0], x0[1], x1[2] });
            var p6 = Vector<double>.Build.DenseOfArray(new double[] { x0[0], x1[1], x1[2] });
            var p7 = Vector<double>.Build.DenseOfArray(new double[] { x1[0], x1[1], x1[2] });

            var points = new Vector<double>[] { p0, p5, p3, p6, p0, p4, p1, p7, p2, p0 };

            for (int i = 0; i < points.Length - 1; i++) {
                var startPoint = points[i];
                var endPoint = points[i + 1];
                var delta = (startPoint - endPoint) / (pointsPerLine + 1);

                for (int j = 0; j < pointsPerLine + 1; j++) {
                    calibrationPoints.Add(startPoint + delta * j);
                }
            }
        }

    }
}