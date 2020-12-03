namespace PingPong.KUKA {
    public class WorkspaceLimits {

        public (double X, double Y, double Z) LowerLimit { get; }

        public (double X, double Y, double Z) UpperLimit { get; }

        public WorkspaceLimits(
            (double Min, double Max) X,
            (double Min, double Max) Y,
            (double Min, double Max) Z
        ) {
            LowerLimit = (X.Min, Y.Min, Z.Min);
            UpperLimit = (X.Max, Y.Max, Z.Max);
        }

        public bool CheckPosition(E6POS position) {
            bool checkX = position.X >= LowerLimit.X && position.X <= UpperLimit.X;
            bool checkY = position.Y >= LowerLimit.Y && position.Y <= UpperLimit.Y;
            bool checkZ = position.Z >= LowerLimit.Z && position.Z <= UpperLimit.Z;
            return checkX && checkY && checkZ;
        }

    }
}
