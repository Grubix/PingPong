using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PingPong.Views {
    public partial class CalibrationWindow : Form {

        private class CalibrationTool {

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
                    //MoveRobotToPoint(robot, calibrationPoints[0], robot.Limits.MaxVelocity.XYZ / 3.0);
                    robot.ForceMoveTo(new E6POS(calibrationPoints[0], robot.Position.ABC), 15.0);

                    for (int i = 0; i < calibrationPoints.Count; i++) {
                        // Move robot to next calibration point and wait
                        //MoveRobotToPoint(robot, calibrationPoints[i], robot.Limits.MaxVelocity.XYZ / 3.0);
                        robot.ForceMoveTo(new E6POS(calibrationPoints[i], robot.Position.ABC), 4.0);

                        // Add robot XYZ position to list
                        var kukaPoint = robot.Position.XYZ;
                        kukaRobotPoints.Add(kukaPoint);

                        // Gen n samples from optitrack system and add average ball position to the list
                        var optiTrackPoint = optiTrack.GetAveragePosition(samplesPerPoint);
                        optiTrackPoints.Add(optiTrackPoint);

                        // Calculate new transformation
                        int progress = i * 100 / (calibrationPoints.Count - 1);
                        transformation = new Transformation(optiTrackPoints, kukaRobotPoints);

                        ProgressChanged?.Invoke(progress, transformation);
                    }

                    //MoveRobotToPoint(robot, robot.HomePosition.XYZ, robot.Limits.MaxVelocity.XYZ / 3.0);
                    //robot.ForceMoveTo(robot.HomePosition, 6.0);
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
                double deltaX = Math.Abs(point[0] - robot.Position.X);
                double deltaY = Math.Abs(point[1] - robot.Position.Y);
                double deltaZ = Math.Abs(point[2] - robot.Position.Z);
                double deltaMax = Math.Max(Math.Max(deltaX, deltaY), deltaZ);

                // Robot Vx=Vy=Vz=Ax=Ay=Az=0 => v(T/2)=Vmax => T=15*(x1-x0)/(8*Vmax)
                double duration = 15.0 * deltaMax / (8.0 * Math.Abs(velocity));

                robot.ForceMoveTo(new E6POS(point, robot.Position.ABC), duration);
            }

            public void Calibrate(KUKARobot robot, int pointsPerLine, int samplesPerPoint) {
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

                (double x0, double y0, double z0) = robot.Limits.LowerWorkspaceLimit;
                (double x1, double y1, double z1) = robot.Limits.UpperWorkspaceLimit;

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

                var points = new[] { p0, p5, p3, p6, p0, p4, p1, p7, p2, p4 };

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

        private const string title = "Calibration";

        private readonly CalibrationTool calibrationTool;

        private KUKARobot selectedRobot;

        public CalibrationWindow(OptiTrackSystem optiTrack, BallData ballData, KUKARobot robot1, KUKARobot robot2) {
            calibrationTool = new CalibrationTool(optiTrack);

            InitializeComponent();

            Text = title;
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);

            BindingList<KUKARobot> robotsList = new BindingList<KUKARobot>();

            robotSelect.DropDown += (s, e) => {
                if (robotsList.Contains(robot1)) {
                    if (!robot1.IsInitialized()) {
                        robotsList.Remove(robot1);
                    }
                } else {
                    if (robot1.IsInitialized()) {
                        robotsList.Add(robot1);
                    }
                }

                if (robotsList.Contains(robot2)) {
                    if (!robot2.IsInitialized()) {
                        robotsList.Remove(robot2);
                    }
                } else {
                    if (robot2.IsInitialized()) {
                        robotsList.Add(robot2);
                    }
                }
            };
            robotSelect.TextChanged += (s, e) => {
                selectedRobot = (KUKARobot)robotSelect.SelectedItem;

                if (selectedRobot != null) {
                    startBtn.Enabled = true;
                } else {
                    startBtn.Enabled = false;
                }
            };
            robotSelect.DataSource = robotsList;
            robotSelect.Text = "- Select robot -";

            FormClosing += (s, e) => calibrationTool.Cancel();
            startBtn.Click += (s, e) => {
                calibrationTool.Calibrate(selectedRobot, (int)intermediatePoints.Value, (int)samplesPerPoint.Value);
            };
            stopBtn.Click += (s, e) => {
                Text = title;
                robotSelect.Enabled = true;
                startBtn.Enabled = true;
                calibrationTool.Cancel();
            };

            calibrationTool.Start += () => {
                UpdateUI(() => {
                    Text = $"{title} (0%)";
                    robotSelect.Enabled = false;
                    startBtn.Enabled = false;
                });
            };
            calibrationTool.ProgressChanged += (progress, transformation) => {
                ballData.SetTransformation(selectedRobot, transformation);
                UpdateUI(() => {
                    Text = $"{title} ({progress}%)";
                    progressBar.Value = progress;
                    m11.Text = transformation[0, 0].ToString("F3");
                    m12.Text = transformation[0, 1].ToString("F3");
                    m13.Text = transformation[0, 2].ToString("F3");
                    m14.Text = transformation[0, 3].ToString("F3");

                    m21.Text = transformation[1, 0].ToString("F3");
                    m22.Text = transformation[1, 1].ToString("F3");
                    m23.Text = transformation[1, 2].ToString("F3");
                    m24.Text = transformation[1, 3].ToString("F3");

                    m31.Text = transformation[2, 0].ToString("F3");
                    m32.Text = transformation[2, 1].ToString("F3");
                    m33.Text = transformation[2, 2].ToString("F3");
                    m34.Text = transformation[2, 3].ToString("F3");

                    m41.Text = transformation[3, 0].ToString("F3");
                    m42.Text = transformation[3, 1].ToString("F3");
                    m43.Text = transformation[3, 2].ToString("F3");
                    m44.Text = transformation[3, 3].ToString("F3");
                });
            };
            calibrationTool.Completed += (transformation) => {
                ballData.SetTransformation(selectedRobot, transformation);
                UpdateUI(() => {
                    robotSelect.Enabled = true;
                    startBtn.Enabled = true;
                });
            };
        }

        private void UpdateUI(Action updateAction) {
            if (InvokeRequired) {
                Action actionWrapper = () => {
                    updateAction.Invoke();
                };

                Invoke(actionWrapper);
                return;
            }

            updateAction.Invoke();
        }
    }
}