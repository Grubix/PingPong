using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class CalibrationWindow : Form {

        private const string title = "Calibration";

        private readonly CalibrationTool calibrationTool;

        public CalibrationWindow(KUKARobot robot1, KUKARobot robot2, OptiTrackSystem optiTrack) {
            InitializeComponent();
            ResetMatrix();
            Text = title;

            BindingList<KUKARobot> robotsList = new BindingList<KUKARobot>();

            //if (robot1.IsInitialized()) {
            //    robotsList.Add(robot1);
            //}

            //if (robot2.IsInitialized()) {
            //    robotsList.Add(robot2);
            //}

            robotsList.Add(robot1);
            robotsList.Add(robot2);

            robotSelect.Text = "KUKA robot";
            robotSelect.DataSource = robotsList;
            robotSelect.DisplayMember = "IP";

            calibrationTool = new CalibrationTool(optiTrack);
            calibrationTool.Started += () => {
                Text = title + " (0%)";
                progressBar.Value = 0;
                robotSelect.Enabled = false;
                startBtn.Enabled = false;
            };
            calibrationTool.ProgressChanged += (progress, transformation) => {
                UpdateUI(() => {
                    Text = title + $" ({progress}%)";
                    progressBar.Value = progress;

                    Matrix<double> rotation = transformation.Rotation;
                    Vector<double> translation = transformation.Translation;

                    m11.Text = rotation[0, 0].ToString("F3");
                    m12.Text = rotation[0, 1].ToString("F3");
                    m13.Text = rotation[0, 2].ToString("F3");
                    m14.Text = translation[0].ToString("F3");

                    m21.Text = rotation[1, 0].ToString("F3");
                    m22.Text = rotation[1, 1].ToString("F3");
                    m23.Text = rotation[1, 2].ToString("F3");
                    m24.Text = translation[1].ToString("F3");

                    m31.Text = rotation[2, 0].ToString("F3");
                    m32.Text = rotation[2, 1].ToString("F3");
                    m33.Text = rotation[2, 2].ToString("F3");
                    m34.Text = translation[2].ToString("F3");

                    m41.Text = (0.0).ToString("F3");
                    m42.Text = (0.0).ToString("F3");
                    m43.Text = (0.0).ToString("F3");
                    m44.Text = (1.0).ToString("F3");
                });
            };
            calibrationTool.Completed += transformation => {
                robotSelect.Enabled = true;
                startBtn.Enabled = true;
                //TODO: co z transformacja ?
            };
        }

        private void ResetMatrix() {
            m11.Text = (1.0).ToString("F3");
            m12.Text = (0.0).ToString("F3");
            m13.Text = (0.0).ToString("F3");
            m14.Text = (0.0).ToString("F3");

            m21.Text = (0.0).ToString("F3");
            m22.Text = (1.0).ToString("F3");
            m23.Text = (0.0).ToString("F3");
            m24.Text = (0.0).ToString("F3");

            m31.Text = (0.0).ToString("F3");
            m32.Text = (0.0).ToString("F3");
            m33.Text = (1.0).ToString("F3");
            m34.Text = (0.0).ToString("F3");

            m41.Text = (0.0).ToString("F3");
            m42.Text = (0.0).ToString("F3");
            m43.Text = (0.0).ToString("F3");
            m44.Text = (1.0).ToString("F3");
        }

        private void StartCalibration(object sender, EventArgs e) {
            KUKARobot selectedRobot = (KUKARobot) robotSelect.SelectedItem;

            //if (!selectedRobot.IsInitialized()) {
            //    throw new InvalidOperationException("Robot is not initialized");
            //}

            calibrationTool.Calibrate(selectedRobot, 50);
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
