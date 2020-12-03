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

        public bool CheckPosition(E6POS position) {
            return WorkspaceLimits.CheckPosition(position);
        }

        public bool CheckAxisPosition(E6AXIS axisPosition) {
            return AxisLimits.CheckAxisPosition(axisPosition);
        }

        public bool CheckAxisVelocity(E6AXIS currentAxisPosition, E6AXIS previousAxisPosition) {
            bool checkA1 = Math.Abs(currentAxisPosition.A1 - previousAxisPosition.A1) <= AxisLimits.MaxVelocity;
            bool checkA2 = Math.Abs(currentAxisPosition.A2 - previousAxisPosition.A2) <= AxisLimits.MaxVelocity;
            bool checkA3 = Math.Abs(currentAxisPosition.A3 - previousAxisPosition.A3) <= AxisLimits.MaxVelocity;
            bool checkA4 = Math.Abs(currentAxisPosition.A4 - previousAxisPosition.A4) <= AxisLimits.MaxVelocity;
            bool checkA5 = Math.Abs(currentAxisPosition.A5 - previousAxisPosition.A5) <= AxisLimits.MaxVelocity;
            bool checkA6 = Math.Abs(currentAxisPosition.A6 - previousAxisPosition.A6) <= AxisLimits.MaxVelocity;

            return checkA1 && checkA2 && checkA3 && checkA4 && checkA5 && checkA6;
        }

        public bool CheckCorrection(E6POS correction) {
            bool checkX = Math.Abs(correction.X) <= MaxCorrection.XYZ;
            bool checkY = Math.Abs(correction.Y) <= MaxCorrection.XYZ;
            bool checkZ = Math.Abs(correction.Z) <= MaxCorrection.XYZ;
            bool checkA = Math.Abs(correction.A) <= MaxCorrection.ABC;
            bool checkB = Math.Abs(correction.B) <= MaxCorrection.ABC;
            bool checkC = Math.Abs(correction.C) <= MaxCorrection.ABC;

            return checkX && checkY && checkZ && checkA && checkB && checkC;
        }

    }
}