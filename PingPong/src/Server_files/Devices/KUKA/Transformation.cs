using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace PingPong.KUKA {
    /// <summary>
    /// Represents transformation between two coordinate systems (A to B)
    /// </summary>
    class Transformation {

        private readonly Matrix<double> rotation;

        public Matrix<double> Rotation {
            get {
                return rotation.Clone();
            }
        }

        private readonly Vector<double> translation;

        public Vector<double> Translation {
            get {
                return translation.Clone();
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
            int detSign = Math.Sign((V * UT).Determinant());

            V[0, 2] *= detSign;
            V[1, 2] *= detSign;
            V[2, 2] *= detSign;

            rotation = V * UT;
            translation = -1 * rotation * centroidB + centroidA;
        }

        public Transformation(Matrix<double> rotation, Vector<double> translation) {
            //TODO: niekoniecznie bedzie potrzebne
        }

        /// <summary>
        /// Converts point in A coordinate system to B coordinate system
        /// </summary>
        /// <param name="pointInA">point in A coordinate system</param>
        /// <returns></returns>
        public Vector<double> ConvertPoint(Vector<double> pointInA) {
            return null; //TODO: Jazda babsonik <3
        }

        //TODO: jakies dwie metody do zapisu i odczytu danych

    }
}
