using MathNet.Numerics.LinearAlgebra;

namespace PingPong.KUKA {
    public abstract class RobotVector {

        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Z { get; protected set; }
        public double A { get; protected set; }
        public double B { get; protected set; }
        public double C { get; protected set; }

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

    }
}
