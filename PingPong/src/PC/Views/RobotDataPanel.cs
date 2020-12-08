using PingPong.KUKA;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Views {
    public partial class RobotDataPanel : UserControl {

        private const double Ts = 80;

        private const int maxSamples = 5000;

        private readonly Stopwatch stopWatch;

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private long deltaTime = 0;

        private Series posX, posY, posZ, posA, posB, posC;

        private Series velX, velY, velZ, velA, velB, velC;

        public RobotDataPanel() {
            InitializeComponent();
            InitializePositionChart();
            InitializeVelocityChart();
            stopWatch = new Stopwatch();
        }

        public void AssignRobot(KUKARobot robot) {
            robot.FrameReceived += f => {
                stopWatch.Stop();
                deltaTime += stopWatch.ElapsedMilliseconds;
                stopWatch.Restart();

                totalSamples++;
                visibleSamples++;

                if (deltaTime < Ts) {
                    return;
                }

                deltaTime = 0;

                UpdateUI(() => {
                    if (visibleSamples >= maxSamples) {
                        visibleSamples = 0;

                        posX.Points.Clear();
                        posY.Points.Clear();
                        posZ.Points.Clear();
                        posA.Points.Clear();
                        posB.Points.Clear();
                        posC.Points.Clear();

                        velX.Points.Clear();
                        velY.Points.Clear();
                        velZ.Points.Clear();
                        velA.Points.Clear();
                        velB.Points.Clear();
                        velC.Points.Clear();

                        positionChart.ChartAreas[0].AxisX.Minimum = totalSamples;
                        velocityChart.ChartAreas[0].AxisX.Minimum = totalSamples;
                        positionChart.ChartAreas[0].AxisX.Maximum = totalSamples + maxSamples;
                        velocityChart.ChartAreas[0].AxisX.Maximum = totalSamples + maxSamples;
                    }

                    var position = robot.Position;
                    var velocity = robot.Velocity;

                    posX.Points.AddXY(totalSamples, position.X);
                    posY.Points.AddXY(totalSamples, position.Y);
                    posZ.Points.AddXY(totalSamples, position.Z);
                    posA.Points.AddXY(totalSamples, position.A);
                    posB.Points.AddXY(totalSamples, position.B);
                    posC.Points.AddXY(totalSamples, position.C);

                    velX.Points.AddXY(totalSamples, velocity.X);
                    velY.Points.AddXY(totalSamples, velocity.Y);
                    velZ.Points.AddXY(totalSamples, velocity.Z);
                    velA.Points.AddXY(totalSamples, velocity.A);
                    velB.Points.AddXY(totalSamples, velocity.B);
                    velC.Points.AddXY(totalSamples, velocity.C);

                    posXText.Text = position.X.ToString("F3");
                    posYText.Text = position.Y.ToString("F3");
                    posZText.Text = position.Z.ToString("F3");
                    posAText.Text = position.A.ToString("F3");
                    posBText.Text = position.B.ToString("F3");
                    posCText.Text = position.C.ToString("F3");

                    velXText.Text = velocity.X.ToString("F3");
                    velYText.Text = velocity.Y.ToString("F3");
                    velZText.Text = velocity.Z.ToString("F3");
                    velAText.Text = velocity.A.ToString("F3");
                    velBText.Text = velocity.B.ToString("F3");
                    velCText.Text = velocity.C.ToString("F3");
                });
            };
        }

        private void InitializePositionChart() {
            posX = new Series {
                Name = "position X",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posY = new Series {
                Name = "position Y",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posZ = new Series {
                Name = "position Z",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posA = new Series {
                Name = "position A",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posB = new Series {
                Name = "position B",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posC = new Series {
                Name = "position C",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            posX.Points.AddXY(0, 0);
            posY.Points.AddXY(0, 0);
            posZ.Points.AddXY(0, 0);
            posA.Points.AddXY(0, 0);
            posB.Points.AddXY(0, 0);
            posC.Points.AddXY(0, 0);

            InitializeCheckBox(positionChart, posX, posXCheck);
            InitializeCheckBox(positionChart, posY, posYCheck);
            InitializeCheckBox(positionChart, posZ, posZCheck);
            InitializeCheckBox(positionChart, posA, posACheck);
            InitializeCheckBox(positionChart, posB, posBCheck);
            InitializeCheckBox(positionChart, posC, posCCheck);

            posXCheck.Checked = true;
            posYCheck.Checked = true;
            posZCheck.Checked = true;

            positionChart.ChartAreas[0].AxisX.Maximum = maxSamples;
        }

        private void InitializeVelocityChart() {
            velX = new Series {
                Name = "velocity X",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velY = new Series {
                Name = "velocity Y",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velZ = new Series {
                Name = "velocity Z",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velA = new Series {
                Name = "velocity A",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velB = new Series {
                Name = "velocity B",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velC = new Series {
                Name = "velocity C",
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            velX.Points.AddXY(0, 0);
            velY.Points.AddXY(0, 0);
            velZ.Points.AddXY(0, 0);
            velA.Points.AddXY(0, 0);
            velB.Points.AddXY(0, 0);
            velC.Points.AddXY(0, 0);

            InitializeCheckBox(velocityChart, velX, velXCheck);
            InitializeCheckBox(velocityChart, velY, velYCheck);
            InitializeCheckBox(velocityChart, velZ, velZCheck);
            InitializeCheckBox(velocityChart, velA, velACheck);
            InitializeCheckBox(velocityChart, velB, velBCheck);
            InitializeCheckBox(velocityChart, velC, velCCheck);

            velXCheck.Checked = true;
            velYCheck.Checked = true;
            velZCheck.Checked = true;

            velocityChart.ChartAreas[0].AxisX.Maximum = maxSamples;
        }

        private void InitializeCheckBox(Chart chart, Series series, CheckBox checkBox) {
            checkBox.CheckedChanged += (s, e) => {
                if (checkBox.Checked && !chart.Series.Contains(series)) {
                    chart.Series.Add(series);
                } else if (chart.Series.Contains(series)) {
                    chart.Series.Remove(series);
                }
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
