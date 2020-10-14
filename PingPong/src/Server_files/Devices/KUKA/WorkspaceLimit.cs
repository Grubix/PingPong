using System;

namespace PingPong.Devices.KUKA {
    class WorkspaceLimit {

        public double LowerX { get; }

        public double LowerY { get; }

        public double LowerZ { get; }

        public double UpperX { get; }

        public double UpperY { get; }

        public double UpperZ { get; }

        public WorkspaceLimit(double lowerX, double lowerY, double lowerZ,
            double upperX, double upperY, double upperZ) {

            if(lowerX < upperX && lowerY < upperY && lowerZ < upperZ) {
                LowerX = lowerX;
                LowerY = lowerY;
                LowerZ = lowerZ;
                UpperX = upperX;
                UpperY = upperY;
                UpperZ = upperZ;
            } else {
                throw new Exception();
            }
        }

        public bool CheckPosition(E6POS position) {
            return
                position.X >= LowerX && position.X <= UpperX &&
                position.Y >= LowerY && position.Y <= UpperY &&
                position.Z >= LowerZ && position.Z <= UpperZ;
        }

    }
}
