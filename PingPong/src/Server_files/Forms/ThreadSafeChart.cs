using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Forms {
    public partial class ThreadSafeChart : UserControl {

        private readonly Stopwatch stopWatch = new Stopwatch();

        private readonly Series series;

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private long deltaTime = 0;

        public int MaxSamples { get; set; }

        public int RefreshTime { get; set; }

        public ThreadSafeChart() {
            InitializeComponent();

            MaxSamples = 1500;
            RefreshTime = 60;

            series = new Series {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };

            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = MaxSamples;

            chart.Series.Add(series);
            stopWatch.Start();
        }

        public void AddPoint(double value) {
            stopWatch.Stop();

            deltaTime += stopWatch.ElapsedMilliseconds;

            stopWatch.Reset();
            stopWatch.Start();

            if (deltaTime < RefreshTime) {
                totalSamples++;
                visibleSamples++;
                return;
            }

            deltaTime = 0;

            ThreadSafeAddPoint threadSafeAddPoint = v => {
                if (visibleSamples++ < MaxSamples) {
                    series.Points.AddXY(totalSamples++, v);
                } else {
                    visibleSamples = 0;

                    for (int i = series.Points.Count - 2; i >= 0; i--) {
                        series.Points.RemoveAt(i);
                    }

                    chart.ChartAreas[0].AxisX.Minimum = totalSamples;
                    chart.ChartAreas[0].AxisX.Maximum = totalSamples + MaxSamples;
                }
            };

            chart.Invoke(threadSafeAddPoint, new object[] { value });
        }

        private delegate void ThreadSafeAddPoint(double value);

    }
}
