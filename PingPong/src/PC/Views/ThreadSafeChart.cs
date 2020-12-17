using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Views {
    public partial class ThreadSafeChart : UserControl {

        private readonly Stopwatch stopWatch = new Stopwatch();

        private readonly Series series1;

        private readonly Series series2;

        private int visibleSamples = 0;

        private long totalSamples = 0;

        private long deltaTime = 0;

        [Description("Max visible samples"), Category("Data")]
        public int MaxSamples { get; set; } = 5000;

        [Description("Time offset in milliseconds between chart updates"), Category("Data")]
        public int RefreshTimeOffset { get; set; }

        public ThreadSafeChart() {
            InitializeComponent();
            MaxSamples = 5000;
            RefreshTimeOffset = 80;

            series1 = new Series {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            series2 = new Series {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3
            };

            chart.Series.Add(series1);
            chart.Series.Add(series2);

            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = MaxSamples;

            series1.Points.AddXY(0, 0);
            series2.Points.AddXY(0, 0);

            stopWatch.Start();
        }

        public void AddPoint(double value1, double value2) {
            stopWatch.Stop();

            deltaTime += stopWatch.ElapsedMilliseconds;

            stopWatch.Reset();
            stopWatch.Start();

            if (deltaTime < RefreshTimeOffset) {
                totalSamples++;
                visibleSamples++;
                return;
            }

            deltaTime = 0;

            ThreadSafeAddPoint threadSafeAddPoint = (v1, v2) => {
                if (visibleSamples++ < MaxSamples) {
                    series1.Points.AddXY(totalSamples, v1);
                    series2.Points.AddXY(totalSamples, v2);
                    totalSamples++;
                } else {
                    visibleSamples = 0;

                    for (int i = series1.Points.Count - 2; i >= 0; i--) {
                        series1.Points.RemoveAt(i);
                    }

                    chart.ChartAreas[0].AxisX.Minimum = totalSamples;
                    chart.ChartAreas[0].AxisX.Maximum = totalSamples + MaxSamples;
                }
            };

            chart.Invoke(threadSafeAddPoint, new object[] { value1, value2 });
        }

        private delegate void ThreadSafeAddPoint(double value1, double value2);

    }
}