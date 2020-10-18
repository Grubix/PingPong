using MathNet.Numerics.LinearAlgebra;
using PingPong.KUKA;
using System;
using System.Collections.Generic;


/// <summary>
/// SPOKO WYJAŚNIENIE http://nghiaho.com/?page_id=671
/// https://en.wikipedia.org/wiki/Wahba%27s_problem
/// https://en.wikipedia.org/wiki/Kabsch_algorithm
/// </summary>

namespace PingPong.OptiTrack {
    class OptiTrackCalibration {

        private Vector<double> centroidKuka;
        private Vector<double> centroidOptiTrack;
        private Vector<double> trans;

        private Matrix<double> matrixH;
        private Matrix<double> matrixR;

        private List<Vector<double>> vectorsFromKuka;
        private List<Vector<double>> vectorsFromOptiTrack;

        private E6POS currentPositionKuka;
        private E6POS currentPositionOptiTrack;

        public OptiTrackCalibration(E6POS currentPositionKuka, E6POS currentPositionOptiTrack) {
            vectorsFromKuka = new List<Vector<double>>();
            vectorsFromOptiTrack = new List<Vector<double>>();

            this.currentPositionKuka = currentPositionKuka;
            this.currentPositionOptiTrack = currentPositionOptiTrack;


            centroidKuka = Vector<double>.Build.Dense(3);
            centroidOptiTrack = Vector<double>.Build.Dense(3);
            trans = Vector<double>.Build.Dense(3);
            //ts = new CancellationTokenSource();

            matrixH = Matrix<double>.Build.Dense(3, 3);
            matrixR = Matrix<double>.Build.Dense(3, 3);

            //UseDefaultCalibration();
        }

        public void AddCalibrationPoints() {
            Vector<double> vectorPointKuka = Vector<double>.Build.Dense(3);
            vectorPointKuka = currentPositionKuka.XYZ;
            vectorsFromKuka.Add(vectorPointKuka);

            Vector<double> positionPointOptiTrack = Vector<double>.Build.Dense(3);
            positionPointOptiTrack.Clear();
            for (int i = 0; i < 200; i++) { // oni biorą po 200 probek jednego pkt  --> trzeba sie dowiedziec jak optiTrack odbiera te pkt

                positionPointOptiTrack[0] += currentPositionOptiTrack.X; //TODO: 
                positionPointOptiTrack[1] += currentPositionOptiTrack.Y;
                positionPointOptiTrack[2] += currentPositionOptiTrack.Z;
                //Thread.Sleep(5);  
                //TODO --> mozna chyba zastąpić poprzez funkcje lock() / Thread.Sleep(5) --> zatrzymuje główny wątek na 5 sek
            }
            positionPointOptiTrack = positionPointOptiTrack / 200;
            vectorsFromOptiTrack.Add(positionPointOptiTrack);
        }

        public void CalculateCentroids() {
            centroidKuka.Clear();
            centroidOptiTrack.Clear();

            foreach (Vector<double> vector in vectorsFromKuka) {
                centroidKuka += vector;
            }

            foreach (Vector<double> vector in vectorsFromOptiTrack) {
                centroidOptiTrack += vector;
            }

            centroidKuka = centroidKuka.Divide(vectorsFromKuka.Count);
            centroidOptiTrack = centroidOptiTrack.Divide(vectorsFromOptiTrack.Count);

        }

        public void CalculateMatrixH() {
            Matrix<double> matrixPointOptiTrackk = Matrix<double>.Build.Dense(3, 1);
            Matrix<double> matrixPointKuka = Matrix<double>.Build.Dense(1, 3);
            Vector<double> pointOptiTrack = Vector<double>.Build.Dense(3);
            Vector<double> pointKuka = Vector<double>.Build.Dense(3);
            matrixPointOptiTrackk.Clear();
            matrixPointKuka.Clear();
            pointOptiTrack.Clear();
            pointKuka.Clear();
            matrixH.Clear();

            for (int i = 0; i < vectorsFromKuka.Count; i++) {
                pointOptiTrack = vectorsFromOptiTrack[i] - centroidOptiTrack;
                pointKuka = vectorsFromKuka[i] - centroidKuka;

                for (int j = 0; j < 3; j++) {
                    matrixPointOptiTrackk[j, 0] = pointOptiTrack[j];
                    matrixPointKuka[0, j] = pointKuka[j];
                }

                matrixH += (matrixPointOptiTrackk * matrixPointKuka);
            }
        }

        public void CalculateRotationAndTranslation() {
            // Rozklad SVD --> matrixH = U*W*VT 
            var SVD = matrixH.Svd(true); // tworzy rozklad wedlug wartosci osobistych macierzy matrixH

            Console.WriteLine(SVD.W); // SVD.W --> wyciaga macierz diagonalna --> posiada nieujemne wartosci osobliwe macierzy matrixH
            matrixR = SVD.VT.Transpose() * SVD.U.Transpose(); // wymnażamy otrzymane macierze VT oraz U, sa to macierze ortogonalne (U^-1=U^T)

            if (matrixR.Determinant() < 0) {
                matrixR[0, 2] *= -1;
                matrixR[1, 2] *= -1;
                matrixR[2, 2] *= -1;
            }

            trans = -1 * matrixR * centroidOptiTrack + centroidKuka;
        }

        public void Calibration() {
            AddCalibrationPoints();
            CalculateCentroids();
            CalculateMatrixH();
            CalculateRotationAndTranslation();
        }
    }
}

