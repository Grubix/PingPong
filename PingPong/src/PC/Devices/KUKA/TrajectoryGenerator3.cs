namespace PingPong.KUKA {
    class TrajectoryGenerator3 {

        private class Polynominal {

            private double k0, k1, k2, k3, k4, k5; // Polynominal coefficients

            private double vn, an; // Next velocity and next acceleration

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x0"></param>
            /// <param name="x1"></param>
            /// <param name="v1"></param>
            /// <param name="T"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            public double GetNextValue(double x0, double x1, double v1, double T, double t) {
                double T1 = T;
                double T2 = T1 * T1;
                double T3 = T1 * T2;
                double T4 = T1 * T3;
                double T5 = T1 * T4;

                k0 = x0;
                k1 = vn;
                k2 = an / 2.0;
                k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * an - 12.0 * T1 * vn - 8.0 * T1 * v1 + 20.0 * (x1 - x0));
                k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * an + 16.0 * T1 * vn + 14.0 * T1 * v1 + 30.0 * (x0 - x1));
                k5 = 1.0 / (2.0 * T5) * (-T2 * an - 6.0 * T1 * (vn + v1) + 12.0 * (x1 - x0));

                double t1 = t;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                double nextValue = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                vn = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                an = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                return nextValue;
            }

            //public double GetMaxVelocity() {
            //    double Tx1 = -k4 / (5.0 * k5);
            //    double Tx2 = Tx1 * Tx1;
            //    double Tx3 = Tx1 * Tx2;
            //    double Tx4 = Tx1 * Tx3;
            //    return 5.0 * k5 * Tx4 + 4.0 * k4 * Tx3 + 3.0 * k3 * Tx2 + 2.0 * k2 * Tx1 + k1;
            //}

        }

        private const double Ts = 0.004;

        private readonly Polynominal PolyX = new Polynominal();

        private readonly Polynominal PolyY = new Polynominal();

        private readonly Polynominal PolyZ = new Polynominal();

        private readonly Polynominal PolyA = new Polynominal();

        private readonly Polynominal PolyB = new Polynominal();

        private readonly Polynominal PolyC = new Polynominal();

        private E6POS targetPosition;

        private double movementDuration;

        private double elapsedTime;

        public (double X, double Y, double Z) CurrentXYZVelocity {
            get {
                return (0, 0, 0);
            }
        }

        public (double A, double B, double C) CurrentABCVelocity {
            get {
                return (0, 0, 0);
            }
        }

        public (double X, double Y, double Z) CurrentXYZAcceleration {
            get {
                return (0, 0, 0);
            }
        }

        public (double A, double B, double C) CurrentABCAcceleration {
            get {
                return (0, 0, 0);
            }
        }

        public TrajectoryGenerator3(E6POS currentPosition) {
            targetPosition = currentPosition;
        }

        //TODO: dorobic wektor do zadawania predkosci koncowej
        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, double movementDuration) {
            if (!targetPosition.Compare(this.targetPosition, 0.01, 0.1) || movementDuration != this.movementDuration) {
                this.targetPosition = targetPosition;
                this.movementDuration = movementDuration;
                elapsedTime = 0.0;
            }

            if (elapsedTime + Ts < this.movementDuration) {
                double timeToDest = this.movementDuration - elapsedTime;
                elapsedTime += Ts;

                return new E6POS(
                    PolyX.GetNextValue(currentPosition.X, targetPosition.X, 0.0, timeToDest, Ts),
                    PolyY.GetNextValue(currentPosition.Y, targetPosition.Y, 0.0, timeToDest, Ts),
                    PolyZ.GetNextValue(currentPosition.Z, targetPosition.Z, 0.0, timeToDest, Ts),
                    PolyA.GetNextValue(currentPosition.A, targetPosition.A, 0.0, timeToDest, Ts),
                    PolyB.GetNextValue(currentPosition.B, targetPosition.B, 0.0, timeToDest, Ts),
                    PolyC.GetNextValue(currentPosition.C, targetPosition.C, 0.0, timeToDest, Ts)
                ) - currentPosition;
            } else {
                return new E6POS();
            }
        }
    }
}
