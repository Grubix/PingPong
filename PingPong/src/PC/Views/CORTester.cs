using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Views {
    public partial class CORTester : Form {

        private readonly List<double> zValues = new List<double>();

        private readonly OptiTrackSystem optiTrack;

        public CORTester(OptiTrackSystem optiTrack) {
            InitializeComponent();
            this.optiTrack = optiTrack;

            startBtn.Click += (s, e) => optiTrack.FrameReceived += ProcessFrame;
            clearBtn.Click += (s, e) => {
                zValues.Clear();
                chart.Series[0].Points.Clear();
                chart.Annotations.Clear();
            };
            calculateBtn.Click += (s, e) => {
                int from = (int) fromSampleInput.Value;
                int to = (int)toSampleInput.Value;

                var peeks = FindPeeks(from, to);
                var CORs = new List<double>();

                for (int i = 0; i < peeks.Count; i++) {
                    if (i >= 1) {
                        CORs.Add(Math.Sqrt(peeks[i] / peeks[i - 1]));
                    }

                    HorizontalLineAnnotation ann = new HorizontalLineAnnotation {
                        Y = peeks[i],
                        AxisY = chart.ChartAreas[0].AxisY,
                        AxisX = chart.ChartAreas[0].AxisX,
                        LineColor = Color.Red,
                        LineWidth = 1,
                        LineDashStyle = ChartDashStyle.Dash,
                        IsInfinitive = true
                    };

                    chart.Annotations.Add(ann);
                }

                double averageCOR = 0.0;

                for (int i = 0; i < CORs.Count; i++) {
                    averageCOR += CORs[i];
                }

                averageCOR /= Math.Max(CORs.Count, 1.0);
                averageCORText.Text = averageCOR.ToString("F3");
            };

            fromSampleInput.ValueChanged += (s, e) => chart.ChartAreas[0].AxisX.Minimum = (int) fromSampleInput.Value;
            toSampleInput.ValueChanged += (s, e) => chart.ChartAreas[0].AxisX.Maximum = (int)toSampleInput.Value;

            int bounces = 6;
            double cor = 0.8;
            double zOnHit = 0.0;
            double z0 = 1;
            double t = 0.04;
            double z = 1;
            double g = 9.81;

            int sample = 0;

            Random rand = new Random();

            while (z > zOnHit) {
                z = z0 - g * t * t / 2.0;
                t += 0.004;

                chart.Series[0].Points.AddXY(sample, z * 1000 + rand.NextDouble() * 32 - 16);
                zValues.Add(z);
                sample++;
            }

            double v = -g * t;

            for (int i = 0; i < bounces; i++) {
                double v0 = Math.Abs(v * cor);

                t = 0.004;
                z = zOnHit + v0 * t - t * t * g / 2.0;
                v = v0 - g * t;

                while (z > zOnHit) {
                    z = zOnHit + v0 * t - t * t * g / 2.0;
                    v = v0 - g * t;
                    t += 0.004;

                    chart.Series[0].Points.AddXY(sample, z * 1000 + rand.NextDouble() * 32 - 16);
                    zValues.Add(z);
                    sample++;
                }
            }

            chart.ChartAreas[0].AxisX.Interval = zValues.Count / 20;

            var peekss = FindPeeks(0, sample);
            var CORss = new List<double>();

            for (int i = 0; i < peekss.Count; i++) {
                HorizontalLineAnnotation ann = new HorizontalLineAnnotation {
                    Y = peekss[i] * 1000,
                    AxisY = chart.ChartAreas[0].AxisY,
                    AxisX = chart.ChartAreas[0].AxisX,
                    LineColor = Color.Red,
                    LineWidth = 1,
                    LineDashStyle = ChartDashStyle.Dash,
                    IsInfinitive = true
                };

                chart.Annotations.Add(ann);
            }
        }

        private List<double> FindPeeks(int fromSample, int toSample, int checkRange = 6) {
            List<double> peeks = new List<double>();

            for (int i = fromSample + checkRange; i < toSample - checkRange; i++) {
                double zValue = zValues[i];

                bool checkDown = true;
                for (int j = i - checkRange; j < i; j++) {
                    checkDown &= zValues[j] <= zValue ;
                }

                bool checkUp = true;
                for (int j = i + 1; j < i + checkRange + 1; j++) {
                    checkUp &= zValues[j] <= zValue ;
                }

                if (checkUp && checkDown) {
                    peeks.Add(zValue);
                }
            }

            return peeks;
        }

        private void ProcessFrame(InputFrame frame) {
            if (frame.Position[2] < 0.0) {
                optiTrack.FrameReceived -= ProcessFrame;
                return;
            }

            zValues.Add(frame.Position[2]);

            fromSampleInput.Maximum = zValues.Count;
            toSampleInput.Maximum = zValues.Count;
            toSampleInput.Value = zValues.Count;

            if (zValues.Count % 5 == 0) {
                UpdateUI(() => {
                    chart.Series[0].Points.AddXY(zValues.Count, frame.Position[2]);
                    chart.ChartAreas[0].AxisX.Interval = zValues.Count / 20;
                });
            }
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
