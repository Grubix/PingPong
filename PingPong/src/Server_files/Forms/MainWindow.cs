using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class MainWindow : Form {

        private readonly KUKARobot robot1;

        private readonly KUKARobot robot2;

        private readonly OptiTrackSystem optiTrack;

        //private IApplication application; //TODO: docelowo po odebraniu ramki z optitracka metoda compute(...)

        private CalibrationWindow calibrationWindow;

        public MainWindow() {
            InitializeComponent();
            InitializeControls();
            robot1 = InitializeRobot1();
            robot2 = InitializeRobot2();
            //optiTrack = InitializeOptiTrackSystem();
        }

        private void InitializeControls() {
            incXBtn.Click += (s, e) => robot1.Shift(new E6POS(50, 0, 0));
            decXBtn.Click += (s, e) => robot1.Shift(new E6POS(-50, 0, 0));

            incYBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 50, 0));
            decYBtn.Click += (s, e) => robot1.Shift(new E6POS(0, -50, 0));

            incZBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 50));
            decZBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, -50));

            incABtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 1, 0, 0));
            decABtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, -1, 0, 0));

            incBBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 1, 0));
            decBBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, -1, 0));

            incCBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 0, 1));
            decCBtn.Click += (s, e) => robot1.Shift(new E6POS(0, 0, 0, 0, 0, -1));

            calibrationBtn.Click += (s, e) => {
                if (calibrationWindow == null || calibrationWindow.IsDisposed) {
                    calibrationWindow = new CalibrationWindow(optiTrack, robot1, robot2);
                }

                calibrationWindow.Show();
                calibrationWindow.Activate();
                calibrationWindow.WindowState = FormWindowState.Normal;
            };
        }

        private KUKARobot InitializeRobot1() {
            RobotLimits limits = new RobotLimits {
                LimitX = (40, 390),
                LimitY = (-100, 250),
                LimitZ = (350, 600),
                LimitA1 = (0, 360),
                LimitA2 = (0, 360),
                LimitA3 = (0, 360),
                LimitA4 = (0, 360),
                LimitA5 = (0, 360),
                LimitA6 = (0, 360),
                LimitCorrection = (0.5, 0.05)
            };

            KUKARobot robot1 = new KUKARobot(8081, limits);

            robot1.FrameReceived += frameReceived => {
                UpdateUI(() => {
                    posXText.Text = frameReceived.Position.X.ToString();
                    posYText.Text = frameReceived.Position.Y.ToString();
                    posZText.Text = frameReceived.Position.Z.ToString();
                    posAText.Text = frameReceived.Position.A.ToString();
                    posBText.Text = frameReceived.Position.B.ToString();
                    posCText.Text = frameReceived.Position.C.ToString();
                });
            };
            robot1.FrameSent += frameSent => {
                realTimeChart.AddPoint(robot1.CurrentPosition.X, robot1.TargetPosition.X);
            };

            robot1.Initialize();

            return robot1;
        }

        private KUKARobot InitializeRobot2() {
            RobotLimits limits = new RobotLimits {
                LimitX = (40, 390),
                LimitY = (-100, 250),
                LimitZ = (350, 600),
                LimitA1 = (0, 360),
                LimitA2 = (0, 360),
                LimitA3 = (0, 360),
                LimitA4 = (0, 360),
                LimitA5 = (0, 360),
                LimitA6 = (0, 360),
                LimitCorrection = (0.5, 0.05)
            };

            KUKARobot robot2 = new KUKARobot(8082, limits);

            robot2.Initialize();

            return robot2;
        }

        private OptiTrackSystem InitializeOptiTrackSystem() {
            OptiTrackSystem optiTrack = new OptiTrackSystem();
            optiTrack.Initialize();

            return optiTrack;
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