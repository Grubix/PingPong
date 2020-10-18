using System;

namespace PingPong.KUKA {
    class RobotLimits {

        private readonly double[] workspaceLowerPoint = new double[3];

        private readonly double[] workspaceUpperPoint = new double[3];

        private readonly double maxXYZCorrection;

        private readonly double maxABCCorrection;

        public RobotLimits(double[] workspaceLowerPoint, double[] workspaceUpperPoint,
            double maxXYZCorrection, double maxABCCorrection) {

            bool invalidWorkspacePoints =
                workspaceLowerPoint.Length != 3 ||
                workspaceUpperPoint.Length != 3 ||
                workspaceLowerPoint[0] > workspaceUpperPoint[0] ||
                workspaceLowerPoint[1] > workspaceUpperPoint[1] ||
                workspaceLowerPoint[2] > workspaceUpperPoint[2];

            if (invalidWorkspacePoints) {
                throw new ArgumentException("Invalid workspace points");
            }

            Array.Copy(workspaceLowerPoint, this.workspaceLowerPoint, 3);
            Array.Copy(workspaceUpperPoint, this.workspaceUpperPoint, 3);
            this.maxXYZCorrection = Math.Abs(maxXYZCorrection);
            this.maxABCCorrection = Math.Abs(maxABCCorrection);
        }

        public bool CheckPosition(E6POS position) {
            return
                position.X >= workspaceLowerPoint[0] && position.X <= workspaceUpperPoint[0] &&
                position.Y >= workspaceLowerPoint[1] && position.Y <= workspaceUpperPoint[1] &&
                position.Z >= workspaceLowerPoint[2] && position.Z <= workspaceUpperPoint[2];
        }

        public bool CheckCorrection(E6POS correction) {
            return
                Math.Abs(correction.X) <= maxXYZCorrection &&
                Math.Abs(correction.Y) <= maxXYZCorrection &&
                Math.Abs(correction.Z) <= maxXYZCorrection &&
                Math.Abs(correction.A) <= maxABCCorrection &&
                Math.Abs(correction.B) <= maxABCCorrection &&
                Math.Abs(correction.C) <= maxABCCorrection;
        }

    }
}
