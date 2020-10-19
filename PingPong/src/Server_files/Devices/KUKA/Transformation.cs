using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace PingPong.KUKA {
    /// <summary>
    /// Represents transformation between two coordinate systems
    /// </summary>
    class Transformation {

        private readonly Matrix<double> rotation;

        private readonly Vector<double> translation;

        /// <summary>
        /// Calculate transformation between two coordinate systems,
        /// basing on <see href="https://en.wikipedia.org/wiki/Kabsch_algorithm">Kabsh algorithm</see>
        /// </summary>
        /// <param name="pointsA">Set of points in A coordinate system</param>
        /// <param name="pointsB">Set of points in B coordinate system</param>
        public Transformation(List<Vector<double>> pointsA, List<Vector<double>> pointsB) {
            if (pointsA.Count != pointsB.Count) {
                throw new ArgumentException("Coś tam po ang. ze liczba punktow musi sie zgadzac");
            }

            //TODO: Jazda babsonik <3
        }

        public Vector<double> ConvertPointToA(Vector<double> pointInB) {
            return null; //TODO: Jazda babsonik <3
        }

        public Vector<double> ConvertPointToB(Vector<double> pointInA) {
            return null; //TODO: Jazda babsonik <3
        }

        //TODO: jakies dwie metody do zapisu i odczytu danych

    }
}
