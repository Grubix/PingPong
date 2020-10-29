using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class CalibrationWindow : Form {

        private const string title = "Calibration";

        private readonly CalibrationTool calibrationTool;

        private KUKARobot selectedRobot;

        public CalibrationWindow(OptiTrackSystem optiTrack, KUKARobot robot1, KUKARobot robot2) {
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
            robotSelect.DisplayMember = "IP";
            robotSelect.DataSource = robotsList;
            robotSelect.Text = "- Select robot -";

            calibrationTool = new CalibrationTool();
            calibrationTool.Started += () => {
                Text = title + " (0%)";
                progressBar.Value = 0;
                robotSelect.Enabled = false;
                startBtn.Enabled = false;
            };
            calibrationTool.ProgressChanged += progress => {
                UpdateUI(() => {
                    Text = title + $" ({progress}%)";
                    progressBar.Value = progress;
                });
            };
            calibrationTool.Completed += transformation => {
                UpdateUI(() => {
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

                //TODO: co z transformacja gdzie ją trzymać ??? w BallData, w Optitracku, w kazdym robocie z osobna ??
            };

            startBtn.Click += (s, e) => {
                if (selectedRobot.IsInitialized()) {
                    calibrationTool.Calibrate(optiTrack, selectedRobot, 50);
                } else {
                    startBtn.Enabled = false;
                }
            };

            FormClosing += (s, e) => calibrationTool.Cancel();
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
