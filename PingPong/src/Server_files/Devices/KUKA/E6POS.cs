using System;

namespace PingPong.Devices.KUKA {
    public class E6POS : KUKAVector, ICloneable {

        private const double XYZComparsionTolerance = 0.00001;

        private const double ABCComparsionTolerance = 0.01;

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public E6POS(double X, double Y, double Z) : this(X, Y, Z, 0, 0, 0) {
        }

        public E6POS() : this(0, 0, 0, 0, 0, 0) {
        }

        public E6POS Clear() {
            return new E6POS();
        }

        public E6POS ClearXYZ() {
            return new E6POS(0, 0, 0, A, B, C);
        }

        public E6POS ClearABC() {
            return new E6POS(X, Y, Z, 0, 0, 0);
        }

        public object Clone() {
            return new E6POS(X, Y, Z, A, B, C);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static E6POS operator + (E6POS pos1, E6POS pos2) {
            return new E6POS(
                pos1.X + pos2.X,
                pos1.Y + pos2.Y,
                pos1.Z + pos2.Z,
                pos1.A + pos2.A,
                pos1.B + pos2.B,
                pos1.C + pos2.C
            );
        }

        public static E6POS operator - (E6POS pos1, E6POS pos2) {
            return new E6POS(
                pos1.X - pos2.X,
                pos1.Y - pos2.Y,
                pos1.Z - pos2.Z,
                pos1.A - pos2.A,
                pos1.B - pos2.B,
                pos1.C - pos2.C
            );
        }

        public static bool operator == (E6POS pos1, E6POS pos2) {
            return
                Math.Abs(pos1.X - pos2.X) <= XYZComparsionTolerance &&
                Math.Abs(pos1.Y - pos2.Y) <= XYZComparsionTolerance &&
                Math.Abs(pos1.Z - pos2.Z) <= XYZComparsionTolerance &&
                Math.Abs(pos1.A - pos2.A) <= ABCComparsionTolerance &&
                Math.Abs(pos1.B - pos2.B) <= ABCComparsionTolerance &&
                Math.Abs(pos1.C - pos2.C) <= ABCComparsionTolerance;
        }

        public static bool operator != (E6POS pos1, E6POS pos2) {
            return !(pos1 == pos2);
        }

    }
}
