using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class CalibrationWindow : Form {

        private const string title = "Calibration";

        private readonly OptiTrackSystem optiTrack;

        private readonly BackgroundWorker worker;

        private KUKARobot selectedRobot;

        public CalibrationWindow(OptiTrackSystem optiTrack, KUKARobot robot1, KUKARobot robot2) {
            this.optiTrack = optiTrack;
            worker = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };

            InitializeComponent();
            ResetMatrix();

            Text = title;
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);

            BindingList<KUKARobot> robotsList = new BindingList<KUKARobot>();

            robotSelect.DropDown += (s, e) => {
                if (robotsList.Contains(robot1)) {
                    if(!robot1.IsInitialized()) {
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
                selectedRobot = (KUKARobot) robotSelect.SelectedItem;

                if (selectedRobot != null) {
                    startBtn.Enabled = true;
                } else {
                    startBtn.Enabled = false;
                }
            };
            robotSelect.DataSource = robotsList;
            robotSelect.Text = "- Select robot -";

            startBtn.Click += (s, e) => Calibrate(50);
            FormClosing += (s, e) => worker.CancelAsync();
        }

        /// <summary>
        /// Finds <see cref="Transformation">transformation</see> between OptiTrack coordinate system 
        /// and selected KUKA robot coordinate system
        /// </summary>
        /// <param name="interPoints">number of intermediate points</param>
        /// <param name="optiTrackSamples">number of optitrack samples per each calibration point</param>
        /// <param name="duration">duration of robot movement (in seconds) between each calibration point</param>
        private void Calibrate(int interPoints, int optiTrackSamples = 200, double duration = 3) {
            if (worker.IsBusy) {
                throw new InvalidOperationException("Calibration in progress");
            }

            if (!optiTrack.IsInitialized()) {
                throw new InvalidOperationException("OptiTrack system is not initialized");
            }

            if (!selectedRobot.IsInitialized()) {
                throw new InvalidOperationException("KUKA robot is not initialized");
            }

            Text = title + " (0%)";
            progressBar.Value = 0;
            robotSelect.Enabled = false;
            startBtn.Enabled = false;

            var kukaPoints = new List<Vector<double>>();
            var optiTrackPoints = new List<Vector<double>>();
            var calibrationPoints = GetCalibrationPoints(selectedRobot.LowerWorkspacePoint, selectedRobot.UpperWorkspacePoint, interPoints);

            void collectPoints(object sender, DoWorkEventArgs args) {
                selectedRobot.ForceMoveTo(calibrationPoints[0], 10);

                for (int i = 0; i < calibrationPoints.Count; i++) {
                    E6POS point = calibrationPoints[i];
                    selectedRobot.ForceMoveTo(point, duration);

                    var kukaPoint = point.XYZ;
                    kukaPoints.Add(kukaPoint);

                    var optiTrackPoint = optiTrack.GetAveragePosition(optiTrackSamples);
                    optiTrackPoints.Add(optiTrackPoint);

                    int progress = i * 100 / (calibrationPoints.Count - 1);

                    UpdateUI(() => {
                        progressBar.Value = progress;
                        Text = title + $" ({progress}%)";
                    });
                }
            }

            worker.DoWork += collectPoints;
            worker.RunWorkerCompleted += (sender, args) => {
                worker.DoWork -= collectPoints;

                if (args.Error != null) {
                    throw args.Error;
                }

                if (!args.Cancelled) {
                    Transformation transformation = new Transformation(optiTrackPoints, kukaPoints);

                    //TODO: co dalej z transformacją, gdzie ją trzymać, gdzie przekazać ?

                    UpdateUI(() => {
                        Text = title;
                        robotSelect.Enabled = true;
                        startBtn.Enabled = true;

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
                }
            };

            worker.RunWorkerAsync();
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

        private void ResetMatrix() {
            m11.Text = (1.0).ToString("F1");
            m12.Text = (0.0).ToString("F1");
            m13.Text = (0.0).ToString("F1");
            m14.Text = (0.0).ToString("F1");

            m21.Text = (0.0).ToString("F1");
            m22.Text = (1.0).ToString("F1");
            m23.Text = (0.0).ToString("F1");
            m24.Text = (0.0).ToString("F1");

            m31.Text = (0.0).ToString("F1");
            m32.Text = (0.0).ToString("F1");
            m33.Text = (1.0).ToString("F1");
            m34.Text = (0.0).ToString("F1");

            m41.Text = (0.0).ToString("F1");
            m42.Text = (0.0).ToString("F1");
            m43.Text = (0.0).ToString("F1");
            m44.Text = (1.0).ToString("F1");
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