using System;

namespace PingPong.KUKA {
    public class RobotLimits {

        public WorkspaceLimits WorkspaceLimits { get; }

        public AxisLimits AxisLimits { get; } 

        public (double XYZ, double ABC) MaxCorrection { get; }

        public (double XYZ, double ABC) MaxVelocity { get; }

        public RobotLimits(WorkspaceLimits workspaceLimits, AxisLimits axisLimits, (double XYZ, double ABC) maxCorrection) {
            WorkspaceLimits = workspaceLimits;
            AxisLimits = axisLimits;
            MaxCorrection = maxCorrection;
            MaxVelocity = (maxCorrection.XYZ / 0.004, maxCorrection.ABC / 0.004);
        }

        public bool CheckPosition(RobotVector position) {
            return WorkspaceLimits.CheckPosition(position);
        }

        public bool CheckVelocity(RobotVector velocity) {
            bool checkX = Math.Abs(velocity.X) <= MaxVelocity.XYZ;
            bool checkY = Math.Abs(velocity.Y) <= MaxVelocity.XYZ;
            bool checkZ = Math.Abs(velocity.Z) <= MaxVelocity.XYZ;
            bool checkA = Math.Abs(velocity.A) <= MaxVelocity.ABC;
            bool checkB = Math.Abs(velocity.B) <= MaxVelocity.ABC;
            bool checkC = Math.Abs(velocity.C) <= MaxVelocity.ABC;

            return checkX && checkY && checkZ && checkA && checkB && checkC;
        }

        public bool CheckCorrection(RobotVector correction) {
            bool checkX = Math.Abs(correction.X) <= MaxCorrection.XYZ;
            bool checkY = Math.Abs(correction.Y) <= MaxCorrection.XYZ;
            bool checkZ = Math.Abs(correction.Z) <= MaxCorrection.XYZ;
            bool checkA = Math.Abs(correction.A) <= MaxCorrection.ABC;
            bool checkB = Math.Abs(correction.B) <= MaxCorrection.ABC;
            bool checkC = Math.Abs(correction.C) <= MaxCorrection.ABC;

            return checkX && checkY && checkZ && checkA && checkB && checkC;
        }

        public bool CheckAxisPosition(AxisPosition axisPosition) {
            return AxisLimits.CheckAxisPosition(axisPosition);
        }

    }
}