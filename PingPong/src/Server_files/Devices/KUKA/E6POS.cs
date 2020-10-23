using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.KUKA {
    public class E6POS : ICloneable {

        private const double XYZComparsionTolerance = 0.001;

        private const double ABCComparsionTolerance = 0.0001;

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

        public override string ToString() {
            return 
                $"X={Math.Round(X * 1000) / 1000}, " +
                $"Y={Math.Round(Y * 1000) / 1000}, " +
                $"Z={Math.Round(Z * 1000) / 1000}, " +
                $"A={Math.Round(A * 1000) / 1000}, " +
                $"B={Math.Round(B * 1000) / 1000}, " +
                $"C={Math.Round(C * 1000) / 1000}";
        }

        public static E6POS operator +(E6POS pos1, E6POS pos2) {
            return new E6POS(
                pos1.X + pos2.X,
                pos1.Y + pos2.Y,
                pos1.Z + pos2.Z,
                pos1.A + pos2.A,
                pos1.B + pos2.B,
                pos1.C + pos2.C
            );
        }

        public static E6POS operator -(E6POS pos1, E6POS pos2) {
            return new E6POS(
                pos1.X - pos2.X,
                pos1.Y - pos2.Y,
                pos1.Z - pos2.Z,
                pos1.A - pos2.A,
                pos1.B - pos2.B,
                pos1.C - pos2.C
            );
        }

        public static bool operator ==(E6POS pos1, E6POS pos2) {
            return
                Math.Abs(pos1.X - pos2.X) <= XYZComparsionTolerance &&
                Math.Abs(pos1.Y - pos2.Y) <= XYZComparsionTolerance &&
                Math.Abs(pos1.Z - pos2.Z) <= XYZComparsionTolerance &&
                Math.Abs(pos1.A - pos2.A) <= ABCComparsionTolerance &&
                Math.Abs(pos1.B - pos2.B) <= ABCComparsionTolerance &&
                Math.Abs(pos1.C - pos2.C) <= ABCComparsionTolerance;
        }

        public static bool operator !=(E6POS pos1, E6POS pos2) {
            return !(pos1 == pos2);
        }

    }
}
