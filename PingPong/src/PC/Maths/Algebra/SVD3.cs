using System;
using System.Collections.Generic;
using System.Linq;

namespace PingPong.Maths {
    /// <summary>
    /// Represents Singular Value decomposition of real matrix using Jacobi method
    /// </summary>
    class SVD3 {

        private const int maxIterations = 100000;

        public Matrix3 U { get; private set; }

        public Matrix3 UT { get; private set; }

        public Matrix3 S { get; private set; }

        public Matrix3 V { get; private set; }

        public Matrix3 VT { get; private set; }

        public SVD3(Matrix3 input, double errorTolerance) {
            S = input.Transpose() * input;
            V = Matrix3.Identity();

            int currentIteration = 0;
            while (!CheckError(S, errorTolerance)) {
                Compute(S);

                if (currentIteration == maxIterations) {
                    break;
                }

                currentIteration++;
            }

            S[0, 0] = Math.Sqrt(S[0, 0]);
            S[1, 1] = Math.Sqrt(S[1, 1]);
            S[2, 2] = Math.Sqrt(S[2, 2]);

            VT = V.Transpose();
            Matrix3 SVT = S * VT;

            if (SVT.Determinant() == 0) {
                U = Matrix3.Identity();
            } else {
                U = input * SVT.Inverse();
            }

            UT = U.Transpose();
        }

        private void Compute(Matrix3 matrix) {
            (int p, int q) = FindPivotPosition(matrix);

            double e = (matrix[q, q] - matrix[p, p]) / (2 * matrix[p, q]);
            double t = Math.Sign(e) / (Math.Abs(e) + Math.Sqrt(e * e + 1));

            double c = 1 / Math.Sqrt(t * t + 1);
            double s = t * c;

            Matrix3 Q = Matrix3.Identity();
            Q[p, p] = c;
            Q[p, q] = s;
            Q[q, p] = -s;
            Q[q, q] = c;

            S = Q.Transpose() * matrix * Q;
            V *= Q;
        }

        private (int p, int q) FindPivotPosition(Matrix3 matrix) {
            var matrixCells = new List<(int p, int q, double value)>();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (i != j) {
                        matrixCells.Add((i, j, matrix[i, j]));
                    }
                }
            }

            (int p, int q, double value) maxValue = matrixCells[0];

            for (int i = 1; i < matrixCells.Count; i++) {
                if (Math.Abs(matrixCells[i].value) > Math.Abs(maxValue.value)) {
                    maxValue = matrixCells[i];
                }
            }

            return (maxValue.p, maxValue.q);
        }

        private bool CheckError(Matrix3 matrix, double tolerance) {
            var offDiagonalValues = new List<double>();
            var diagonalValues = new List<double>();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (i == j) {
                        diagonalValues.Add(matrix[i, j]);
                    } else {
                        offDiagonalValues.Add(matrix[i, j]);
                    }
                }
            }

            return offDiagonalValues.Max() / diagonalValues.Max() <= tolerance;
        }

    }
}
