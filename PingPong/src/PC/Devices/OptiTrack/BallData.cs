using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using PingPong.Maths;
using System.Collections.Generic;

namespace PingPong.OptiTrack {
    public class BallData {

        private Dictionary<string, Transformation> transformations;

        private Vector<double> position;

        public BallData() {
            transformations = new Dictionary<string, Transformation>();
            position = Vector<double>.Build.Dense(3);
        }

        public void Update(InputFrame receivedFrame) {
            position = receivedFrame.Position;
        }

        public void SetTransformation(KUKARobot robot, Transformation transformation) {
            transformations[robot.Ip] = transformation;
        }

        public Vector<double> GetPosition(KUKARobot robot) {
            return transformations[robot.Ip].Convert(position);
        }

    }
}