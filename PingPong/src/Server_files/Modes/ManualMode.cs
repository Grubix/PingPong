﻿using PingPong.Devices;

namespace PingPong.Modes {
    class ManualMode : IMode {

        public E6POS TargetPosition { get; set; }

        public void Compute(KUKARobot robot1) {
            robot1.TargetPosition = TargetPosition;
        }

    }
}