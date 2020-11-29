using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace PingPong.Maths {
    class Polyfit {

        // polynomial coefficients
        private Vector<double> Xcoeff;
        private Vector<double> Ycoeff;
        private Vector<double> Zcoeff;

        // ball positions lists
        private List<double> X = new List<double>();
        private List<double> Y = new List<double>();
        private List<double> Z = new List<double>();
        private List<double> time = new List<double>();
        // max number of points to prediction, rather even (pol. 'parzyste')
        readonly private int maxPointsCount = 100;

        // prediction
        private double Xpred;
        private double Ypred;
        private double Tpred;
        private Vector<double> prediction;

        private double Zlevel;

        public bool go = false;

        public Polyfit(double Zlevel) {
            this.Zlevel = Zlevel;
            prediction = Vector<double>.Build.Dense(3);
        }

        public void AddNewPosition(double X, double Y, double Z, double time) {
            if (X > -390 && X < 700 && Y > -100 && Y < 700 && Z > 100 && Z < 1000) {
                if (this.X.Count >= maxPointsCount)
                {
                    go = true;

                    for (int i = 0; i < maxPointsCount / 2; i++)
                    {
                        this.X[i] = this.X[2 * i];
                        this.Y[i] = this.Y[2 * i];
                        this.Z[i] = this.Z[2 * i];
                        this.time[i] = this.time[2 * i];
                    }

                    this.X.RemoveRange(maxPointsCount / 2, maxPointsCount / 2);
                    this.Y.RemoveRange(maxPointsCount / 2, maxPointsCount / 2);
                    this.Z.RemoveRange(maxPointsCount / 2, maxPointsCount / 2);
                    this.time.RemoveRange(maxPointsCount / 2, maxPointsCount / 2);
                }
                this.X.Add(X);
                this.Y.Add(Y);
                this.Z.Add(Z);
                this.time.Add(time);

                if (this.X.Count > 2) {
                    Xcoeff = GetCoeff(this.X, 1);
                    Ycoeff = GetCoeff(this.Y, 1);
                    Zcoeff = GetCoeff(this.Z, 2);
                }
            } else {
                this.X.Clear();
                this.Y.Clear();
                this.Z.Clear();
                this.time.Clear();
            }
        }

        public Vector<double> GetPrediction() {
            if (X.Count > 2) {
                CountPrediction();
                prediction[0] = Xpred;
                prediction[1] = Ypred;
                prediction[2] = Tpred;
            }

            return prediction;
        }

        // position - ball position in one of XYZ axis, order of polynomial: XY: 1, Z: 2
        private Vector<double> GetCoeff(List<double> position, int order) {

            if (time.Count != position.Count) {
                throw new Exception("Diffrent arrays sizes!");
            }

            // create T matrix
            var T = Matrix<double>.Build.Dense(time.Count, order + 1);
            for (int row = 0; row < time.Count; row++) {
                double val = 1.0;
                for (int col = 0; col < order + 1; col++) {
                    T[row, col] = val;
                    val *= time[row];
                }
            }

            // create Y vector
            var P = Vector<double>.Build.Dense(position.Count);
            for (int i = 0; i < position.Count; i++) {
                P[i] = position[i];
            }

            return (T.Transpose() * T).Inverse() * T.Transpose() * P;
        }

        private void CountPrediction() {
            Tpred = GetEquationResult();
            Xpred = Xcoeff[1] * Tpred + Xcoeff[0];
            Ypred = Ycoeff[1] * Tpred + Ycoeff[0];
        }

        // results -1 if delta < 0
        private double GetEquationResult() {
            double delta = Zcoeff[1] * Zcoeff[1] - 4 * Zcoeff[2] * (Zcoeff[0] - Zlevel);
            if (delta < 0.0)
                return -1;
            return (-Zcoeff[1] - Math.Sqrt(delta)) / 2 / Zcoeff[2];
        }

        public void Clear()
        {
            X.Clear();
            Y.Clear();
            Z.Clear();
            time.Clear();
        }

        public Vector<double> GetX() {
            return Xcoeff;
        }
        public Vector<double> GetY() {
            return Ycoeff;
        }
        public Vector<double> GetZ() {
            return Zcoeff;
        }
    }
}