using MathNet.Numerics.LinearAlgebra;
using PingPong.Forms;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 596.5;

        private KUKARobot robot;

        private Polyfit polyfit;

        private Vector<double> prevPrediction;

        private double timeElapsed;

        private ThreadSafeChart threadSafeChart1;

        public Ping(KUKARobot robot, ThreadSafeChart threadSafeChart1) {
            this.robot = robot;
            polyfit = new Polyfit(Zlevel);
            this.threadSafeChart1 = threadSafeChart1;
        }

        public void ProcessData(BallData ballData) {
            // Pozycja piłeczki (W TEORII XD) w układzie robota przekazanego do konstruktora
            Vector<double> position = ballData.GetPosition(robot);

            polyfit.AddNewPosition(position[0], position[1], position[2], timeElapsed);
            var prediction = polyfit.GetPrediction();
            var collisionPoint = Vector<double>.Build.DenseOfArray(new double[] {
                prediction[0], prediction[1], Zlevel
            });

            threadSafeChart1.AddPoint(prediction[0], prediction[1]);

            System.Console.WriteLine(prediction);
            if (polyfit.go)
                robot.MoveTo(new E6POS(collisionPoint, robot.CurrentPosition.ABC), 5);
            timeElapsed += 0.004;
        }

    }
}
