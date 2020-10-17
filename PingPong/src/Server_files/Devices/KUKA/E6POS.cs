using System;
using MathNet.Numerics.LinearAlgebra;

namespace PingPong.KUKA {
    public class E6POS : ICloneable {

        private const int XYZPrecision = 100000;
        private const int ABCPrecision = 100000;

        private const double XYZComparsionTolerance = 1 / XYZPrecision;
        private const double ABCComparsionTolerance = 1 / ABCPrecision;

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double A { get; }
        public double B { get; }
        public double C { get; }

        public Vector<double> XYZ {
            get {
                return Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z });
            }
        }
        public Vector<double> ABC {
            get {
                return Vector<double>.Build.DenseOfArray(new double[] { A, B, C });
            }
        }
        public Vector<double> XYZABC {
            get {
                return Vector<double>.Build.DenseOfArray(new double[] { X, Y, Z, A, B, C });
            }
        }

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = Math.Round(X * XYZPrecision) / XYZPrecision;
            this.Y = Math.Round(Y * XYZPrecision) / XYZPrecision;
            this.Z = Math.Round(Z * XYZPrecision) / XYZPrecision;
            this.A = Math.Round((A < 0 ? 360.0 + A : A) * ABCPrecision) / ABCPrecision;
            this.B = Math.Round((B < 0 ? 360.0 + B : B) * ABCPrecision) / ABCPrecision;
            this.C = Math.Round((C < 0 ? 360.0 + C : C) * ABCPrecision) / ABCPrecision;
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
