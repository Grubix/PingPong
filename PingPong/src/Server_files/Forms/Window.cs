using PingPong.KUKA;
using PingPong.OptiTrack;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class Window : Form {

        private readonly KUKARobot robot1;

        //private readonly KUKARobot robot2;

        private readonly OptiTrackSystem optiTrack;

        //private ITask task; //TODO: docelowo po odebraniu ramki z optitracka wywołanie metody ComputeTargetPosition()

        public Window() {
            InitializeComponent();

            // Punkty w jakims innym ukladzie
            //new double[] { 50.0, -160.0, 635.0 },
            //new double[] { 590.0, 300.0, 870.0 },

            robot1 = new KUKARobot(8081, new RobotLimits {
                WorkspaceLowerPoint = new double[] { 40.0, -100.0, 350.0 },
                WorkspaceUpperPoint = new double[] { 390.0, 250.0, 600.0 },
                MaxXYZCorrection = 0.5,
                MaxABCCorrection = 0.05
            });

            robot1.Initialized += InitializeControls;
            robot1.FrameReceived += UpdateRobotPosition;
            robot1.FrameSent += fs => realTimeChart.AddPoint(robot1.CurrentPosition.X, robot1.TargetPosition.X);

            optiTrack = new OptiTrackSystem();

            robot1.Initialize();
            optiTrack.Initialize();
        }

        private void InitializeControls() {
            incXBtn.Click += CalibrationTest;
            //incXBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(50, 0, 0));
            decXBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(-50, 0, 0));

            incYBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 50, 0));
            decYBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, -50, 0));

            incZBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 50));
            decZBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, -50));

            incABtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, 1, 0, 0));
            decABtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, -1, 0, 0));

            incBBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, 0, 1, 0));
            decBBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, 0, -1, 0));

            incCBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, 0, 0, 1));
            decCBtn.Click += (s, e) => robot1.ShiftBy(new E6POS(0, 0, 0, 0, 0, -1));

            incXBtn.Enabled = incYBtn.Enabled = incZBtn.Enabled = incABtn.Enabled = incBBtn.Enabled = incCBtn.Enabled = true;
            decXBtn.Enabled = decYBtn.Enabled = decZBtn.Enabled = decABtn.Enabled = decBBtn.Enabled = decCBtn.Enabled = true;
        }

        private void UpdateRobotPosition(KUKA.InputFrame inputFrame) {
            posXText.Text = inputFrame.Position.X.ToString();
            posYText.Text = inputFrame.Position.Y.ToString();
            posZText.Text = inputFrame.Position.Z.ToString();
            posAText.Text = inputFrame.Position.A.ToString();
            posBText.Text = inputFrame.Position.B.ToString();
            posCText.Text = inputFrame.Position.C.ToString();
        }

        private void CalibrationTest(object sender, System.EventArgs e) {
            Task.Run(() => {
                robot1.ForceShiftBy(new E6POS(0, 50, 0));
                System.Console.WriteLine(optiTrack.GetAveragePosition(200));

                robot1.ForceShiftBy(new E6POS(0, 0, -50));
                System.Console.WriteLine(optiTrack.GetAveragePosition(200));

                robot1.ForceShiftBy(new E6POS(0, -50, 0));
                System.Console.WriteLine(optiTrack.GetAveragePosition(200));

                robot1.ForceShiftBy(new E6POS(0, 0, 50));
                System.Console.WriteLine(optiTrack.GetAveragePosition(200));
            }).Wait();
        }

        //TODO: 
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
