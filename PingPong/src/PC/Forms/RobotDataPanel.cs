using PingPong.KUKA;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Forms {
    public partial class RobotDataPanel : UserControl {

        private const double Ts = 80;

        private const int maxSamples = 5000;

        private readonly Stopwatch stopWatch;

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private long deltaTime = 0;

        private Series sx, sy, sz, sa, sb, sc;

        public RobotDataPanel() {
            InitializeComponent();
            InitializePositionChart();
            stopWatch = new Stopwatch();
        }

        public void AssignKUKARobot(KUKARobot robot) {
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

                        sx.Points.Clear();
                        sy.Points.Clear();
                        sz.Points.Clear();
                        sa.Points.Clear();
                        sb.Points.Clear();
                        sc.Points.Clear();

                        positionChart.ChartAreas[0].AxisX.Minimum = totalSamples;
                        positionChart.ChartAreas[0].AxisX.Maximum = totalSamples + maxSamples;
                    }

                    var position = robot.Position;

                    sx.Points.AddXY(totalSamples, position.X);
                    sy.Points.AddXY(totalSamples, position.Y);
                    sz.Points.AddXY(totalSamples, position.Z);
                    sa.Points.AddXY(totalSamples, position.A);
                    sb.Points.AddXY(totalSamples, position.B);
                    sc.Points.AddXY(totalSamples, position.C);

                    posX.Text = position.X.ToString("F3");
                    posY.Text = position.Y.ToString("F3");
                    posZ.Text = position.Z.ToString("F3");
                    posA.Text = position.A.ToString("F3");
                    posB.Text = position.B.ToString("F3");
                    posC.Text = position.C.ToString("F3");
                });
            };
        }

        private void InitializePositionChart() {
            sx = new Series {
                Name = "position X",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sy = new Series {
                Name = "position Y",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sz = new Series {
                Name = "position Z",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sa = new Series {
                Name = "position A",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sb = new Series {
                Name = "position B",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sc = new Series {
                Name = "position C",
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 3
            };

            sx.Points.AddXY(0, 0);
            sy.Points.AddXY(0, 0);
            sz.Points.AddXY(0, 0);
            sa.Points.AddXY(0, 0);
            sb.Points.AddXY(0, 0);
            sc.Points.AddXY(0, 0);

            InitializeCheckBox(positionChart, sx, posXCheck);
            InitializeCheckBox(positionChart, sy, posYCheck);
            InitializeCheckBox(positionChart, sz, posZCheck);
            InitializeCheckBox(positionChart, sa, posACheck);
            InitializeCheckBox(positionChart, sb, posBCheck);
            InitializeCheckBox(positionChart, sc, posCCheck);

            posXCheck.Checked = true;
            posYCheck.Checked = true;
            posZCheck.Checked = true;

            positionChart.ChartAreas[0].AxisX.Maximum = maxSamples;
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
