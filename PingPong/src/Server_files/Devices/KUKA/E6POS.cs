using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.Devices {
    public class E6POS : ICloneable {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

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

        public E6POS() {

        }

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public void Reset() {
            X = Y = Z = A = B = C = 0;
        }

        public object Clone() {
            return new E6POS(X, Y, Z, A, B, C);
        }

        public static E6POS operator + (E6POS pos1, E6POS pos2) {
            return new E6POS() {
                X = pos1.X + pos2.X,
                Y = pos1.Y + pos2.Y,
                Z = pos1.Z + pos2.Z,
                A = pos1.A + pos2.A,
                B = pos1.B + pos2.B,
                C = pos1.C + pos2.C
            };
        }

        public static E6POS operator - (E6POS pos1, E6POS pos2) {
            return new E6POS() {
                X = pos1.X - pos2.X,
                Y = pos1.Y - pos2.Y,
                Z = pos1.Z - pos2.Z,
                A = pos1.A - pos2.A,
                B = pos1.B - pos2.B,
                C = pos1.C - pos2.C
            };
        }

    }
}
