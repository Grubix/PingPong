using MathNet.Numerics.LinearAlgebra;
using PingPong.Views;
using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;

namespace PingPong.Applications {
    class Ping : IApplication {

        private const double Zlevel = 283;

        private readonly KUKARobot robot;

        private readonly Polyfit polyfit;

        //private Vector<double> prevPrediction;

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
            System.Console.WriteLine(polyfit.GetPrediction()[0]);
            System.Console.WriteLine("***** ***");

            (double LowerX, double LowerY, double LowerZ) = robot.Limits.LowerWorkspaceLimit;
            (double UpperX, double UpperY, double UpperZ) = robot.Limits.UpperWorkspaceLimit;
            if (LowerX < prediction[0] && UpperX > prediction[0]
             && LowerY < prediction[1] && UpperY > prediction[1])
                //robot.MoveTo(new E6POS(collisionPoint, robot.Position.ABC), 5);
            timeElapsed += 0.00416;
        }

    }
}
