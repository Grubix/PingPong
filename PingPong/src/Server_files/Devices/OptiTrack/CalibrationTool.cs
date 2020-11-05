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
                robot.ForceMoveTo(new E6POS(calibrationPoints[0], robot.CurrentPosition.ABC), 15);

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

                robot.ForceMoveTo(robot.HomePosition, 15);
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

            var startPoint = robot.LowerWorkspacePoint;
            var endPoint = robot.UpperWorkspacePoint;

            // Shrink workspace by 5mm
            var offset = Vector<double>.Build.DenseOfArray(new double[] {
                endPoint[0] > startPoint[0] ? 5.0 : -5.0,
                endPoint[1] > startPoint[1] ? 5.0 : -5.0,
                endPoint[2] > startPoint[2] ? 5.0 : -5.0
            });

            startPoint += offset;
            endPoint -= offset;

            // Cubid points
            var p0 = Vector<double>.Build.DenseOfArray(new double[] { startPoint[0], startPoint[1], startPoint[2] });
            var p1 = Vector<double>.Build.DenseOfArray(new double[] { endPoint[0], startPoint[1], startPoint[2] });
            var p2 = Vector<double>.Build.DenseOfArray(new double[] { startPoint[0], endPoint[1], startPoint[2] });
            var p3 = Vector<double>.Build.DenseOfArray(new double[] { endPoint[0], endPoint[1], startPoint[2] });
            var p4 = Vector<double>.Build.DenseOfArray(new double[] { startPoint[0], startPoint[1], endPoint[2] });
            var p5 = Vector<double>.Build.DenseOfArray(new double[] { endPoint[0], startPoint[1], endPoint[2] });
            var p6 = Vector<double>.Build.DenseOfArray(new double[] { startPoint[0], endPoint[1], endPoint[2] });
            var p7 = Vector<double>.Build.DenseOfArray(new double[] { endPoint[0], endPoint[1], endPoint[2] });

            AddCalibrationPoints(p0, p5, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p5, p3, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p3, p6, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p6, p0, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p0, p4, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p4, p1, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p1, p7, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p7, p2, pointsPerLine, calibrationPoints);
            AddCalibrationPoints(p2, p0, pointsPerLine, calibrationPoints);
        }

        private void AddCalibrationPoints(Vector<double> startPoint, Vector<double> endPoint, 
            int pointsPerLine, List<Vector<double>> points) {

            var offset = (endPoint - startPoint) / (pointsPerLine + 1);

            for (int i = 0; i < pointsPerLine + 1; i++) {
                calibrationPoints.Add(startPoint + offset * i);
            }
        }

    }
}