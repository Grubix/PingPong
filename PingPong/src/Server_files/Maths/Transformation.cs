using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace PingPong.Maths {
    /// <summary>
    /// Represents 4x4 transformation matrix between two coordinate systems (A to B)
    /// </summary>
    class Transformation {

        private readonly Matrix<double> rotationMatrix;

        public Matrix<double> RotationMatrix {
            get {
                return rotationMatrix.Clone();
            }
        }

        private readonly Vector<double> translationVector;

        public Vector<double> TranslationVector {
            get {
                return translationVector.Clone();
            }
        }

        private readonly Matrix<double> matrix;

        public Matrix<double> Matrix {
            get {
                return matrix.Clone();
            }
        }

        /// <summary>
        /// Gets the value of <see cref="Matrix"></see> at the given row and column
        /// </summary>
        /// <param name="i">row</param>
        /// <param name="j">column</param>
        /// <returns></returns>
        public double this[int i, int j] {
            get {
                return matrix[i, j];
            }
        }

        /// <summary>
        /// Calculate transformation between two coordinate systems (A to B),
        /// basing on <see href="https://en.wikipedia.org/wiki/Kabsch_algorithm">Kabsh algorithm</see>
        /// </summary>
        /// <param name="pointsA">Set of points in A coordinate system</param>
        /// <param name="pointsB">Set of points in B coordinate system</param>
        public Transformation(List<Vector<double>> pointsA, List<Vector<double>> pointsB) {
            if (pointsA.Count != pointsB.Count) {
                throw new ArgumentException("Coś tam po ang. ze liczba punktow musi sie zgadzac");
            }

            int pointsCount = pointsA.Count;
            var centroidA = Vector<double>.Build.Dense(3);
            var centroidB = Vector<double>.Build.Dense(3);

            foreach (var point in pointsA) {
                centroidA += point;
            }

            foreach (var point in pointsB) {
                centroidB += point;
            }

            centroidA /= pointsCount;
            centroidB /= pointsCount;

            // Covariance matrix
            var matrixH = Matrix<double>.Build.Dense(3, 3);

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    double value = 0;

                    for (int k = 0; k < pointsCount; k++) {
                        value += (pointsA[k] - centroidA)[i] * (pointsB[k] - centroidB)[j];
                    }

                    matrixH[i, j] = value;
                }
            }

            var svdDecomposition = matrixH.Svd();
            var UT = svdDecomposition.U.Transpose();
            var V = svdDecomposition.VT.Transpose();

            if ((V * UT).Determinant() <= 0) { //TODO: niedokonca jasne czy ma byc < 0 czy <= 0,
                V[0, 2] *= -1;
                V[1, 2] *= -1;
                V[2, 2] *= -1;
            }

            rotationMatrix = V * UT;
            translationVector = -1 * rotationMatrix * centroidA + centroidB;

            matrix = Matrix<double>.Build.DenseOfArray(new double[,] {
                { rotationMatrix[0, 0], rotationMatrix[0, 1], rotationMatrix[0, 2], translationVector[0] },
                { rotationMatrix[1, 0], rotationMatrix[1, 1], rotationMatrix[1, 2], translationVector[1] },
                { rotationMatrix[2, 0], rotationMatrix[2, 1], rotationMatrix[2, 2], translationVector[2] },
                { 0.0, 0.0, 0.0, 1.0 }
            });
        }

        /// <summary>
        /// Converts point in A coordinate system to point B coordinate system
        /// </summary>
        /// <param name="pointInA">point in A coordinate system</param>
        /// <returns></returns>
        public Vector<double> ConvertPoint(Vector<double> pointInA) {
            return rotationMatrix * pointInA + translationVector;
        }

    }
}
