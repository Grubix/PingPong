using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System.Collections.Generic;

namespace PingPong.OptiTrack {
    public class BallData {

        private Dictionary<KUKARobot, Transformation> transformations;

        private Vector<double> position;

        private Vector<double> velocity;

        public BallData() {
            transformations = new Dictionary<KUKARobot, Transformation>();
            position = Vector<double>.Build.Dense(3);
            velocity = Vector<double>.Build.Dense(3);
        }

        public void Update(InputFrame receivedFrame) {
            position = receivedFrame.Position;
        }

        public void SetTransformation(KUKARobot robot, Transformation transformation) {
            transformations[robot] = transformation;
        }

        public Vector<double> GetPosition(KUKARobot robot) {
            return transformations[robot].Convert(position);
        }

        public Vector<double> GetVelocity(KUKARobot robot) {
            return transformations[robot].Convert(velocity);
        }

    }
}