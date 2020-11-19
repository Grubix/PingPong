using PingPong.KUKA;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Forms {
    public partial class RobotDataPanel : UserControl {

        private const double Ts = 100;

        private const int maxSamples = 5000;

        private readonly Stopwatch stopWatch;

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private long deltaTime = 0;

        private Series sx, sy, sz, sa, sb, sc;

        private Series vx, vy, vz, va, vb, vc;

        public RobotDataPanel() {
            InitializeComponent();
            InitializePositionChart();
            InitializeVelocityChart();
            stopWatch = new Stopwatch();
        }

        public void SetKUKARobot(KUKARobot robot) {
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

                        vx.Points.Clear();
                        vy.Points.Clear();
                        vz.Points.Clear();
                        va.Points.Clear();
                        vb.Points.Clear();
                        vc.Points.Clear();

                        positionChart.ChartAreas[0].AxisX.Minimum = totalSamples;
                        velocityChart.ChartAreas[0].AxisX.Minimum = totalSamples;
                        positionChart.ChartAreas[0].AxisX.Maximum = totalSamples + maxSamples;
                        velocityChart.ChartAreas[0].AxisX.Maximum = totalSamples + maxSamples;
                    }

                    var position = robot.CurrentPosition;
                    var (Vx, Vy, Vz) = robot.CurrentXYZVelocity;
                    var (Va, Vb, Vc) = robot.CurrentABCVelocity;

                    sx.Points.AddXY(totalSamples, position.X);
                    sy.Points.AddXY(totalSamples, position.Y);
                    sz.Points.AddXY(totalSamples, position.Z);
                    sa.Points.AddXY(totalSamples, position.A);
                    sb.Points.AddXY(totalSamples, position.B);
                    sc.Points.AddXY(totalSamples, position.C);

                    vx.Points.AddXY(totalSamples, Vx);
                    vy.Points.AddXY(totalSamples, Vy);
                    vz.Points.AddXY(totalSamples, Vz);
                    va.Points.AddXY(totalSamples, Va);
                    vb.Points.AddXY(totalSamples, Vb);
                    vc.Points.AddXY(totalSamples, Vc);

                    posX.Text = position.X.ToString("F3");
                    posY.Text = position.Y.ToString("F3");
                    posZ.Text = position.Z.ToString("F3");
                    posA.Text = position.A.ToString("F3");
                    posB.Text = position.B.ToString("F3");
                    posC.Text = position.C.ToString("F3");

                    velX.Text = Vx.ToString("F3");
                    velY.Text = Vy.ToString("F3");
                    velZ.Text = Vz.ToString("F3");
                    velA.Text = Va.ToString("F3");
                    velB.Text = Vb.ToString("F3");
                    velC.Text = Vc.ToString("F3");
                });
            };

            HorizontalLineAnnotation minVelAnnotation = new HorizontalLineAnnotation {
                AxisX = velocityChart.ChartAreas[0].AxisX,
                AxisY = velocityChart.ChartAreas[0].AxisY,
                IsSizeAlwaysRelative = false,
                AnchorY = -robot.MaxXYZVelocity,
                IsInfinitive = true,
                ClipToChartArea = velocityChart.ChartAreas[0].Name,
                LineColor = Color.Red,
                LineWidth = 1,
                LineDashStyle = ChartDashStyle.DashDot
            };

            HorizontalLineAnnotation maxVelAnnotation = new HorizontalLineAnnotation {
                AxisX = velocityChart.ChartAreas[0].AxisX,
                AxisY = velocityChart.ChartAreas[0].AxisY,
                IsSizeAlwaysRelative = false,
                AnchorY = robot.MaxXYZVelocity,
                IsInfinitive = true,
                ClipToChartArea = velocityChart.ChartAreas[0].Name,
                LineColor = Color.Blue,
                LineWidth = 1,
                LineDashStyle = ChartDashStyle.DashDot
            };

            velocityChart.Annotations.Add(minVelAnnotation);
            velocityChart.Annotations.Add(maxVelAnnotation);
        }

        private void InitializePositionChart() {
            sx = new Series {
                Name = "position X",
                ChartType = SeriesChartType.FastLine
            };

            sy = new Series {
                Name = "position Y",
                ChartType = SeriesChartType.FastLine
            };

            sz = new Series {
                Name = "position Z",
                ChartType = SeriesChartType.FastLine
            };

            sa = new Series {
                Name = "position A",
                ChartType = SeriesChartType.FastLine
            };

            sb = new Series {
                Name = "position B",
                ChartType = SeriesChartType.FastLine
            };

            sc = new Series {
                Name = "position C",
                ChartType = SeriesChartType.FastLine
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

        private void InitializeVelocityChart() {
            vx = new Series {
                Name = "velocity X",
                ChartType = SeriesChartType.FastLine
            };

            vy = new Series {
                Name = "velocity Y",
                ChartType = SeriesChartType.FastLine
            };

            vz = new Series {
                Name = "velocity Z",
                ChartType = SeriesChartType.FastLine
            };

            va = new Series {
                Name = "velocity A",
                ChartType = SeriesChartType.FastLine
            };

            vb = new Series {
                Name = "velocity B",
                ChartType = SeriesChartType.FastLine
            };

            vc = new Series {
                Name = "velocity C",
                ChartType = SeriesChartType.FastLine
            };

            vx.Points.AddXY(0, 0);
            vy.Points.AddXY(0, 0);
            vz.Points.AddXY(0, 0);
            va.Points.AddXY(0, 0);
            vb.Points.AddXY(0, 0);
            vc.Points.AddXY(0, 0);

            InitializeCheckBox(velocityChart, vx, velXCheck);
            InitializeCheckBox(velocityChart, vy, velYCheck);
            InitializeCheckBox(velocityChart, vz, velZCheck);
            InitializeCheckBox(velocityChart, va, velACheck);
            InitializeCheckBox(velocityChart, vb, velBCheck);
            InitializeCheckBox(velocityChart, vc, velCCheck);

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
