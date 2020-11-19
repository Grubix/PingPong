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

                // Move robot to first calibration point and wait
                MoveRobotToPoint(robot, calibrationPoints[0], robot.MaxXYZVelocity / 3.0);

                for (int i = 0; i < calibrationPoints.Count; i++) {
                    // Move robot to next calibration point and wait
                    MoveRobotToPoint(robot, calibrationPoints[i], robot.MaxXYZVelocity / 3.0);

                    // Add robot XYZ position to list
                    var kukaPoint = robot.CurrentPosition.XYZ;
                    kukaRobotPoints.Add(kukaPoint);

                    // Gen n samples from optitrack system and add average position of the ball to list
                    var optiTrackPoint = optiTrack.GetAveragePosition(samplesPerPoint);
                    optiTrackPoints.Add(optiTrackPoint);

                    // Calculate new transformation
                    int progress = i * 100 / (calibrationPoints.Count - 1);
                    transformation = new Transformation(optiTrackPoints, kukaRobotPoints);

                    ProgressChanged?.Invoke(progress, transformation);
                }

                MoveRobotToPoint(robot, robot.HomePosition, robot.MaxXYZVelocity / 3.0);
            };

            worker.RunWorkerCompleted += (s, e) => {
                if (e.Error != null) {
                    throw e.Error;
                } else {
                    Completed?.Invoke(transformation);
                }
            };
        }

        private void MoveRobotToPoint(KUKARobot robot, Vector<double> point, double velocity) {
            // Find greatest XYZ displacement
            double deltaX = Math.Abs(point[0] - robot.CurrentPosition.X);
            double deltaY = Math.Abs(point[1] - robot.CurrentPosition.Y);
            double deltaZ = Math.Abs(point[2] - robot.CurrentPosition.Z);
            double deltaMax = Math.Max(Math.Max(deltaX, deltaY), deltaZ);

            // Robot Vx=Vy=Vz=Ax=Ay=Az=0 => v(T/2)=Vmax => T=15*(x1-x0)/(8*Vmax)
            double duration = 15.0 * deltaMax / (8.0 * Math.Abs(velocity));
            robot.ForceMoveTo(new E6POS(point, robot.CurrentPosition.ABC), duration);
        }

        private void MoveRobotToPoint(KUKARobot robot, E6POS position, double velocity) {
            var point = Vector<double>.Build.DenseOfArray(new double[] {
                position.X, position.Y, position.Z
            });

            MoveRobotToPoint(robot, point, velocity);
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

            (double x0, double y0, double z0) = robot.LowerWorkspacePoint;
            (double x1, double y1, double z1) = robot.UpperWorkspacePoint;

            // Shrink workspace by 5mm
            x0 += x1 > x0 ? 5.0 : -5.0;
            x1 -= x1 > x0 ? 5.0 : -5.0;
            y0 += y1 > y0 ? 5.0 : -5.0;
            y1 -= y1 > y0 ? 5.0 : -5.0;
            z0 += z1 > z0 ? 5.0 : -5.0;
            z1 -= z1 > z0 ? 5.0 : -5.0;

            // Workspace vertices
            (double x, double y, double z) p0 = (x0, y0, z0);
            (double x, double y, double z) p1 = (x1, y0, z0);
            (double x, double y, double z) p2 = (x0, y1, z0);
            (double x, double y, double z) p3 = (x1, y1, z0);
            (double x, double y, double z) p4 = (x0, y0, z1);
            (double x, double y, double z) p5 = (x1, y0, z1);
            (double x, double y, double z) p6 = (x0, y1, z1);
            (double x, double y, double z) p7 = (x1, y1, z1);

            var points = new[] { p0, p5, p3, p6, p0, p4, p1, p7, p2, p0 };

            for (int i = 0; i < points.Length - 1; i++) {
                var startPoint = points[i];
                var endPoint = points[i + 1];

                var deltaX = (endPoint.x - startPoint.x) / (pointsPerLine + 1);
                var deltaY = (endPoint.y - startPoint.y) / (pointsPerLine + 1);
                var deltaZ = (endPoint.z - startPoint.z) / (pointsPerLine + 1);

                for (int j = 0; j < pointsPerLine + 1; j++) {
                    calibrationPoints.Add(Vector<double>.Build.DenseOfArray(new double[] {
                        startPoint.x + deltaX * j,
                        startPoint.y + deltaY * j,
                        startPoint.z + deltaZ * j
                    }));
                }
            }
        }

    }
}