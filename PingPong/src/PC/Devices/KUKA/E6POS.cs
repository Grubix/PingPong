using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.KUKA {
    public class E6POS {

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

        public E6POS(double X, double Y, double Z) : this(X, Y, Z, 0.0, 0.0, 0.0) {
        }

        public E6POS(Vector<double> XYZ, Vector<double> ABC) : this(XYZ[0], XYZ[1], XYZ[2], ABC[0], ABC[1], ABC[2]) {
        }

        public E6POS(double X, double Y, double Z, Vector<double> ABC) : this(X, Y, Z, ABC[0], ABC[1], ABC[2]) {
        }

        public E6POS(Vector<double> XYZ, double A, double B, double C) : this(XYZ[0], XYZ[1], XYZ[2], A, B, C) {
        }

        public E6POS() : this(0.0, 0.0, 0.0, 0.0, 0.0, 0.0) {
        }

        public bool Compare(E6POS position, double xyzTolerance, double abcTolerance) {
            return
                Math.Abs(X - position.X) <= xyzTolerance &&
                Math.Abs(Y - position.Y) <= xyzTolerance &&
                Math.Abs(Z - position.Z) <= xyzTolerance &&
                Math.Abs(A - position.A) <= abcTolerance &&
                Math.Abs(B - position.B) <= abcTolerance &&
                Math.Abs(C - position.C) <= abcTolerance;
        }

        public override string ToString() {
            return $"[X={X:F3}, Y={Y:F3}, Z={Z:F3}, A={A:F3}, B={B:F3}, C={C:F3}]";
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

    }
}
