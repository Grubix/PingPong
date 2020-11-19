using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System.Collections.Generic;

namespace PingPong.OptiTrack {
    public class BallData {

        //TODO: beda potrzebne locki ??

        private Dictionary<KUKARobot, Transformation> transformations;

        private Vector<double> position;

        private Vector<double> velocity;

        public BallData() {
            transformations = new Dictionary<KUKARobot, Transformation>();
            position = Vector<double>.Build.Dense(3);
            velocity = Vector<double>.Build.Dense(3);
        }

        public void SetTransformation(KUKARobot robot, Transformation transformation) {
            transformations[robot] = transformation;
        }

        public void Update(InputFrame receivedFrame) {
            //TODO: odpalane w MainWindow.cs po otrzymaniu ramki z optitracka
            //TODO: apdejt predkosci, przyspieszenie, predykcja nastepnej pozycji etc.
            //TODO: tutaj moze sie przydac timer zeby liczyc predkosc!

        }

        public Vector<double> GetPosition(KUKARobot robot) {
            return transformations[robot].Convert(position);
        }

        public Vector<double> GetVelocity(KUKARobot robot) {
            return transformations[robot].Convert(velocity);
        }

    }
}