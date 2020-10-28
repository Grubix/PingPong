using PingPong.KUKA;
using PingPong.Maths;
using PingPong.OptiTrack;

namespace PingPong.Applications {
    class Ping : IApplication {

        private readonly KUKARobot robot;

        /// <summary>
        /// optiTrack coordinate system to robot coordinate system transformation
        /// </summary>
        private readonly Transformation transformation;

        public Ping(KUKARobot robot, Transformation transformation) {
            this.robot = robot;
            this.transformation = transformation;
        }

        public void Process(BallData ballData) {
        }

        public void Start() {
        }

        public void Stop() {
        }

    }
}
