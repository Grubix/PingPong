using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class Window : Form {

        private readonly KUKARobot robot1;

        private readonly KUKARobot robot2;

        private readonly OptiTrackSystem optiTrack;

        //private ITask task; //TODO: docelowo po odebraniu ramki z optitracka wywołanie metody ComputeTargetPosition()

        public Window() {
            InitializeComponent();
            InitializeControls();
            InitializeRobot1();
            InitializeRobot2();
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
        }

        private void InitializeRobot1() {
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

        private void CalibrationTest(object sender, System.EventArgs e) {
            List<E6POS> calibrationPoints = GetCalibrationPoints(
                robot1.CurrentPosition, 
                robot1.CurrentPosition + new E6POS(150, 0, 0),
                20
            );

            var kukaPoints = new List<Vector<double>>();
            var optiTrackPoints = new List<Vector<double>>();

            Task.Run(() => {
                foreach (E6POS point in calibrationPoints) {
                    robot1.ForceMoveTo(point);
                    kukaPoints.Add(point.XYZ);
                    optiTrackPoints.Add(optiTrack.GetAveragePosition(200));
                }
            });

            // Transformacja z ukladu optitracka do kuki
            Transformation transformation = new Transformation(optiTrackPoints, kukaPoints);
        }

        private List<E6POS> GetCalibrationPoints(E6POS startPosition, E6POS endPosition, uint intermediatePoints) {
            List<E6POS> points = new List<E6POS>();
            uint totalPoints = 2 + intermediatePoints;

            E6POS deltaPosition = new E6POS(
                (endPosition.X - startPosition.X) / (intermediatePoints + 1),
                (endPosition.Y - startPosition.Y) / (intermediatePoints + 1),
                (endPosition.Z - startPosition.Z) / (intermediatePoints + 1),
                (endPosition.A - startPosition.A) / (intermediatePoints + 1),
                (endPosition.B - startPosition.B) / (intermediatePoints + 1),
                (endPosition.C - startPosition.C) / (intermediatePoints + 1)
            );

            for (int i = 0; i < totalPoints; i++) {
                points.Add(startPosition + new E6POS(
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
