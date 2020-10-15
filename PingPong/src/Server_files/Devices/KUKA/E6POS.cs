using System;

namespace PingPong.KUKA {
    public class E6POS : RobotVector, ICloneable {

        private const double XYZComparsionTolerance = 0.00001;

        private const double ABCComparsionTolerance = 0.01;

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A % 180.0;
            this.B = B % 180.0;
            this.C = C % 180.0; //TODO: to modulo moze byc problemem dlaczego generator trajektorii srednio dziala dla abc
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

        public override string ToString() {
            return $"X={X}, Y={Y}, Z={Z}, A={A}, B={B}, C={C}";
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
