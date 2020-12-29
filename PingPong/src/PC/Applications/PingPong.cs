using PingPong.KUKA;
using PingPong.OptiTrack;
using System;

namespace PingPong.Applications {
    class PingPong : IApplication {

        private readonly KUKARobot robot1;

        private readonly KUKARobot robot2;

        private readonly OptiTrackSystem optiTrack;

        public PingPong(KUKARobot robot1, KUKARobot robot2, OptiTrackSystem optiTrack) {
            this.robot1 = robot1;
            this.robot2 = robot2;
            this.optiTrack = optiTrack;
        }

        public void Start() {
            if (!robot1.IsInitialized() || !robot2.IsInitialized() || !optiTrack.IsInitialized()) {
                throw new InvalidOperationException("Both robots and optiTrack system must be initialized");
            }

            // tutaj jakis prosty kodzik odpowiadający za odbijanie piłeczki przez roboty :3
        }
    }
}
