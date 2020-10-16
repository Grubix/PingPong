using System;

namespace PingPong.KUKA {
    public class E6POS : RobotVector, ICloneable {

        private const double XYZComparsionTolerance = 0.01;

        private const double ABCComparsionTolerance = 0.001;

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = Math.Round(X * 10000) / 10000;
            this.Y = Math.Round(Y * 10000) / 10000;
            this.Z = Math.Round(Z * 10000) / 10000;
            this.A = A % 180.0;
            this.B = B % 180.0;

            //this.C = Math.Abs(C % 360.0) * Math.Sign(C); //TODO: to modulo moze byc problemem dlaczego generator trajektorii srednio dziala dla abc
            this.C = C + 180.0;
            /*if (this.C < 0) {
                this.C = 360.0 + this.C;
            }*/
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

       /* public E6POS modulation() {
            this.X = Math.Round(X * 10000) / 10000;
            this.Y = Math.Round(Y * 10000) / 10000;
            this.Z = Math.Round(Z * 10000) / 10000;
        }*/

    }
}
