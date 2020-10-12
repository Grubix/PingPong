using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using PingPong.Devices.KUKA;

namespace PingPong.Devices.OptiTrack
{
    class OptiTrackCalibration
    {
        private Vector<double> centroidKuka;
        private Vector<double> centroidOptiTrack;
        private Vector<double> trans;

        private Matrix<double> matrixH;
        private Matrix<double> matrixR;

        private List<Vector<double>> vectorsKuka;
        private List<Vector<double>> vectorsOptiTrack;

        private E6POS currentPositionKuka;
        private E6POS currentPositionOptiTrack;

        public OptiTrackCalibration(E6POS currentPositionKuka, E6POS currentPositionOptiTrack) {
            vectorsKuka = new List<Vector<double>>();
            vectorsOptiTrack = new List<Vector<double>>();

            this.currentPositionKuka = currentPositionKuka;
            this.currentPositionOptiTrack = currentPositionOptiTrack;


            centroidKuka = Vector<double>.Build.Dense(3);
            centroidOptiTrack = Vector<double>.Build.Dense(3);
            trans = Vector<double>.Build.Dense(3);
            trans.Clear();
            //ts = new CancellationTokenSource();

            matrixH = Matrix<double>.Build.Dense(3, 3);
            matrixH.Clear();
            matrixR = Matrix<double>.Build.Dense(3, 3);
            matrixR.Clear();

            //UseDefaultCalibration();
        }

        public void AddCalibrationPoints() {
            Vector<double> vectorPointKuka = Vector<double>.Build.Dense(3);
            vectorPointKuka = currentPositionKuka.XYZ;
            vectorsKuka.Add(vectorPointKuka);

            Vector<double> positionPointOptiTrack = Vector<double>.Build.Dense(3);
            positionPointOptiTrack.Clear();
            for (int i = 0; i < 200; i++) { // oni biorą po 200 probek jednego pkt  --> trzeba sie dowiedziec jak optiTrack odbiera te pkt

                positionPointOptiTrack[0] += currentPositionOptiTrack.X; //TODO: 
                positionPointOptiTrack[1] += currentPositionOptiTrack.Y;
                positionPointOptiTrack[2] += currentPositionOptiTrack.Z;
                Thread.Sleep(5); // --> mozna chyba zastąpić poprzez funkcje lock() / Thread.Sleep(5) --> zatrzymuje główny wątek na 5 sek
            }
            positionPointOptiTrack = positionPointOptiTrack / 200;
            vectorsOptiTrack.Add(positionPointOptiTrack);
        }
    }
}
