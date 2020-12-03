using PingPong.KUKA;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 283;

        private readonly KUKARobot robot;

        private readonly Chart chart;

        private double timeElapsed;

        public Ping(KUKARobot robot, Chart chart) {
            this.robot = robot;
            this.chart = chart;
        }

        public void ProcessData(OptiTrack.InputFrame data) {
            var position = data.Position; //pozycja w optitracku

            // ... 

            timeElapsed += data.FrameDeltaTime; //FrameDeltaTime - okolo 0.004s
        }

    }
}
