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
            startBtn.Click += (s, e) => calibrationTool.Calibrate(selectedRobot);
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
                UpdateUI(() => {
                    Text = $"{title} ({progress}%)";
                    progressBar.Value = progress;
                    m11.Text = transformation[0, 0].ToString();
                    m12.Text = transformation[0, 1].ToString();
                    m13.Text = transformation[0, 2].ToString();
                    m14.Text = transformation[0, 3].ToString();

                    m21.Text = transformation[1, 0].ToString();
                    m22.Text = transformation[1, 1].ToString();
                    m23.Text = transformation[1, 2].ToString();
                    m24.Text = transformation[1, 3].ToString();

                    m31.Text = transformation[2, 0].ToString();
                    m32.Text = transformation[2, 1].ToString();
                    m33.Text = transformation[2, 2].ToString();
                    m34.Text = transformation[2, 3].ToString();

                    m41.Text = transformation[3, 0].ToString();
                    m42.Text = transformation[3, 1].ToString();
                    m43.Text = transformation[3, 2].ToString();
                    m44.Text = transformation[3, 3].ToString();
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