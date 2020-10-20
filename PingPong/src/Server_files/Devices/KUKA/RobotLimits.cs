using System;

namespace PingPong.KUKA {
    class RobotLimits {

        private double[] workspaceLowerPoint = new double[3];
        public double[] WorkspaceLowerPoint { 
            get {
                return workspaceLowerPoint.Clone() as double[];
            } 
            set {
                if (value.Length != 3) {
                    throw new ArgumentException("Invalid workspace point length");
                }

                workspaceLowerPoint = value.Clone() as double[];
            }
        }

        private double[] workspaceUpperPoint = new double[3];
        public double[] WorkspaceUpperPoint {
            get {
                return workspaceUpperPoint.Clone() as double[];
            }
            set {
                if (value.Length != 3) {
                    throw new ArgumentException("Invalid workspace point length");
                }

                workspaceUpperPoint = value.Clone() as double[];
            }
        }

        private double maxXYZCorrection;
        public double MaxXYZCorrection { 
            get {
                return maxXYZCorrection;
            }
            set {
                maxXYZCorrection = Math.Abs(value);
            } 
        }

        private double maxABCCorrection;
        public double MaxABCCorrection {
            get {
                return maxABCCorrection;
            }
            set {
                maxABCCorrection = Math.Abs(value);
            }
        }

        public bool CheckPosition(E6POS position) {
            return
                position.X >= workspaceLowerPoint[0] && position.X <= workspaceUpperPoint[0] &&
                position.Y >= workspaceLowerPoint[1] && position.Y <= workspaceUpperPoint[1] &&
                position.Z >= workspaceLowerPoint[2] && position.Z <= workspaceUpperPoint[2];
        }

        public bool CheckCorrection(E6POS correction) {
            return
                Math.Abs(correction.X) <= MaxXYZCorrection &&
                Math.Abs(correction.Y) <= MaxXYZCorrection &&
                Math.Abs(correction.Z) <= MaxXYZCorrection &&
                Math.Abs(correction.A) <= MaxABCCorrection &&
                Math.Abs(correction.B) <= MaxABCCorrection &&
                Math.Abs(correction.C) <= MaxABCCorrection;
        }

    }
}
