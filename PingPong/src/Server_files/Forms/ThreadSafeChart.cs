using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Forms {
    public partial class ThreadSafeChart : UserControl {

        private readonly Stopwatch stopWatch = new Stopwatch();

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private long deltaTime = 0;

        public Series Series { get; }

        public int MaxSamples { get; set; } = 1500;

        public int RefreshTime { get; set; } = 60;

        public ThreadSafeChart() {
            InitializeComponent();

            Series = new Series {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2
            };

            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Maximum = MaxSamples;

            chart.Series.Add(Series);
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
                    Series.Points.AddXY(totalSamples++, v);
                } else {
                    visibleSamples = 0;

                    for (int i = Series.Points.Count - 2; i >= 0; i--) {
                        Series.Points.RemoveAt(i);
                    }

                    //chart.ChartAreas[0].AxisX.Minimum = (totalSamples / MaxSamples) * MaxSamples;
                    //chart.ChartAreas[0].AxisX.Maximum = (totalSamples / MaxSamples + 1) * MaxSamples;
                    chart.ChartAreas[0].AxisX.Minimum = totalSamples;
                    chart.ChartAreas[0].AxisX.Maximum = totalSamples + MaxSamples;
                }
            };

            chart.Invoke(threadSafeAddPoint, new object[] { value });
        }

        private delegate void ThreadSafeAddPoint(double value);

    }
}
