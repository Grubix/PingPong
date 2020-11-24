using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator2 {

        private class Polynominal {

            private const double Ts = 0.004;

            private double k0, k1, k2, k3, k4, k5;

            private double x1, v1, T, elapsedTime;

            /// <summary>
            /// Current position
            /// </summary>
            public double X { get; private set; }

            /// <summary>
            /// Current velocity
            /// </summary>
            public double V { get; private set; }

            /// <summary>
            /// Current acceleration
            /// </summary>
            public double A { get; private set; }

            public Polynominal(double currentPosition) {
                x1 = currentPosition;
            }

            public double GetNextValue(double currentPosition, double targetPosition, double duration, double velocity) {                
                if (duration != T || targetPosition != x1 || velocity != v1) {
                    Restart(duration, currentPosition, targetPosition, velocity);
                    elapsedTime = Ts;
                }

                if (elapsedTime >= T) {
                    V = A = 0.0;
                    return currentPosition;
                }

                double t1 = elapsedTime;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                double nextValue = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                V = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                A = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                elapsedTime += Ts;

                return nextValue;
            }

            private void Restart(double T, double x0, double x1, double v1) {
                this.x1 = x1;
                this.v1 = v1;
                this.T = T;

                double T1 = T;
                double T2 = T1 * T1;
                double T3 = T1 * T2;
                double T4 = T1 * T3;
                double T5 = T1 * T4;

                k0 = x0;
                k1 = V;
                k2 = A / 2.0;
                k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * A - 12.0 * T1 * V - 8.0 * T1 * v1 + 20.0 * (x1 - x0));
                k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * A + 16.0 * T1 * V + 14.0 * T1 * v1 + 30.0 * (x0 - x1));
                k5 = 1.0 / (2.0 * T5) * (-T2 * A - 6.0 * T1 * (V + v1) + 12.0 * (x1 - x0));

                // Prymitywny algorytm do znajdowania nowego czasu
                //while (Math.Abs(CalculateMaxVelocity()) > 125.0) {
                //    T1 = T1 + 0.1 * T1;
                //    T2 = T1 * T1;
                //    T3 = T1 * T2;
                //    T4 = T1 * T3;
                //    T5 = T1 * T4;

                //    k0 = x0;
                //    k1 = V;
                //    k2 = A / 2.0;
                //    k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * A - 12.0 * T1 * V - 8.0 * T1 * v1 + 20.0 * (x1 - x0));
                //    k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * A + 16.0 * T1 * V + 14.0 * T1 * v1 + 30.0 * (x0 - x1));
                //    k5 = 1.0 / (2.0 * T5) * (-T2 * A - 6.0 * T1 * (V + v1) + 12.0 * (x1 - x0));
                //}

                //this.T = T1;
            }

            private double CalculateMaxVelocity() {
                // v(Tx) = Vmax <=> v''(Tx) = 0 => Tx = -1/5 * k4/k5

                double Tx1 = -k4 / (5.0 * k5);
                double Tx2 = Tx1 * Tx1;
                double Tx3 = Tx1 * Tx2;
                double Tx4 = Tx1 * Tx3;

                return 5.0 * k5 * Tx4 + 4.0 * k4 * Tx3 + 3.0 * k3 * Tx2 + 2.0 * k2 * Tx1 + k1;
            }

        }

        private readonly Polynominal PolyX;

        private readonly Polynominal PolyY;

        private readonly Polynominal PolyZ;

        private readonly Polynominal PolyA;

        private readonly Polynominal PolyB;

        private readonly Polynominal PolyC;

        public TrajectoryGenerator2(E6POS currentPosition) {
            PolyX = new Polynominal(currentPosition.X);
            PolyY = new Polynominal(currentPosition.Y);
            PolyZ = new Polynominal(currentPosition.Z);
            PolyA = new Polynominal(currentPosition.A);
            PolyB = new Polynominal(currentPosition.B);
            PolyC = new Polynominal(currentPosition.C);
        }

        public (double X, double Y, double Z) CurrentXYZVelocity {
            get {
                return (PolyX.V, PolyY.V, PolyZ.V);
            }
        }

        public (double A, double B, double C) CurrentABCVelocity {
            get {
                return (PolyA.V, PolyB.V, PolyC.V);
            }
        }

        public (double X, double Y, double Z) CurrentXYZAcceleration {
            get {
                return (PolyX.A, PolyY.A, PolyZ.A);
            }
        }

        public (double A, double B, double C) CurrentABCAcceleration {
            get {
                return (PolyA.A, PolyB.A, PolyC.A);
            }
        }

        //TODO: predkosc tutaj chyba powinna byc wektorem ??
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="duration"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public E6POS GetNextPosition(E6POS currentPosition, E6POS targetPosition, double duration, double velocity = 0.0) {
            return new E6POS(
                PolyX.GetNextValue(currentPosition.X, targetPosition.X, duration, velocity),
                PolyY.GetNextValue(currentPosition.Y, targetPosition.Y, duration, velocity),
                PolyZ.GetNextValue(currentPosition.Z, targetPosition.Z, duration, velocity),
                PolyA.GetNextValue(currentPosition.A, targetPosition.A, duration, velocity),
                PolyB.GetNextValue(currentPosition.B, targetPosition.B, duration, velocity),
                PolyC.GetNextValue(currentPosition.C, targetPosition.C, duration, velocity)
            );
        }

        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, double duration, double velocity = 0.0) {
            return GetNextPosition(currentPosition, targetPosition, duration, velocity) - currentPosition;
        }

    }
}
