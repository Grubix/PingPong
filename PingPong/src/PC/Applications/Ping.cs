using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 25.0;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfit;

        private double timeElapsed;

        public Ping(KUKARobot robot) {
            this.robot = robot;
            polyfit = new Polyfit(Zlevel);
        }

        public void ProcessData(BallData ballData) {
            // Pozycja piłeczki (W TEORII XD) w układzie robota przekazanego do konstruktora
            Vector<double> position = ballData.GetPosition(robot);

            System.Console.WriteLine(position);

            polyfit.AddNewPosition(position[0], position[1], position[2], timeElapsed);
            var prediction = polyfit.GetPrediction();
            var collisionPoint = Vector<double>.Build.DenseOfArray(new double[] {
                prediction[0], prediction[1], Zlevel
            });

            robot.MoveTo(new E6POS(collisionPoint, robot.CurrentPosition.ABC), prediction[2]);
            timeElapsed += 0.004;
        }

    }
}
