using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace PingPong.Maths {
    /// <summary>
    /// https://mathworld.wolfram.com/LeastSquaresFittingPolynomial.html
    /// </summary>
    class Polyfit2 {

        public readonly List<double> xValues;

        public readonly List<double> yValues;

        private readonly int order;

        public int PointCount {
            get {
                return xValues.Count;
            }
        }

        public Polyfit2(int order) {
            this.order = order;
            xValues = new List<double>();
            yValues = new List<double>();
        }

        public void AddPoint(double x, double y) {
            xValues.Add(x);
            yValues.Add(y);
        }

        public void Clear() {
            xValues.Clear();
            yValues.Clear();
        }

        public List<double> CalculateCoefficients() {
            var coefficients = new List<double>();

            // Vandermonde matrix
            var X = Matrix<double>.Build.Dense(xValues.Count, order + 1);

            for (int i = 0; i < X.RowCount; i++) {
                X[i, 0] = 1.0;
                for (int j = 1; j < X.ColumnCount; j++) {
                    X[i, j] = X[i, j-1] * xValues[i];
                }
            }

            var XT = X.Transpose();
            var XTX = XT * X;

            if (XTX.Determinant() == 0.0) {
                for (int i = 0; i < order + 1; i++) {
                    coefficients.Add(0.0);
                }

                return coefficients;
            }

            // y values vector
            var Y = Matrix<double>.Build.Dense(yValues.Count, 1);

            for (int i = 0; i < Y.RowCount; i++) {
                Y[i, 0] = yValues[i];
            }

            // polynominal coefficients vector
            var C = XTX.Inverse() * XT * Y;

            for (int i = 0; i < C.RowCount; i++) {
                coefficients.Add(C[i, 0]);
            }

            return coefficients;
        }

    }
}
