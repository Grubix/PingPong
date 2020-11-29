using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator5 {

        private class Polynominal {

            private double k0, k1, k2, k3, k4, k5; // Polynominal coefficients

            private double xn, vn, an; // Next value, velocity and next acceleration

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

                xn = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                vn = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                an = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                return xn;
            }

        }

        private const double Ts = 0.004;

        private readonly Polynominal PolyX = new Polynominal();

        private readonly Polynominal PolyY = new Polynominal();

        private readonly Polynominal PolyZ = new Polynominal();

        private readonly Polynominal PolyA = new Polynominal();

        private readonly Polynominal PolyB = new Polynominal();

        private readonly Polynominal PolyC = new Polynominal();

        private E6POS targetPosition;

        private double targetDuration;

        private double elapsedTime;

        public TrajectoryGenerator5(E6POS currentPosition) {
            targetPosition = currentPosition;
            targetDuration = 0.0;
            elapsedTime = 0.0;
        }

        //TODO: wektor predkosci koncowej
        public void SetTargetPosition(E6POS targetPosition, double targetDuration) {
            bool targetPositionChanged = !targetPosition.Compare(this.targetPosition, 0.01, 0.1);
            bool movementDurationChanged = Math.Abs(targetDuration - this.targetDuration) > 1e-5;

            if (targetPositionChanged || movementDurationChanged) {
                this.targetPosition = targetPosition;
                this.targetDuration = targetDuration;
                elapsedTime = 0.0;
            }
        }

        //TODO: wektor predkosci koncowej
        public E6POS GetNextCorrection(E6POS currentPosition) {
            if (elapsedTime + Ts < targetDuration) {
                double timeToDest = targetDuration - elapsedTime;
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