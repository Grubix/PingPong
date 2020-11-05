using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator2 {

        private class Polynominal {

            private double a0, a1, a2, a3, a4, a5;

            public double startVelocity;

            public double startAcceleration;

            public void UpdateCoefficients(double currentValue, double targetValue, double targetDuration) {
                double d1 = targetDuration;
                double d2 = d1 * d1;
                double d3 = d1 * d2;
                double d4 = d1 * d3;
                double d5 = d1 * d4;

                a0 = currentValue;
                a1 = startVelocity;
                a2 = startAcceleration / 2;
                a3 = 1 / (2 * d3) * (-3 * startAcceleration * d2 - d1 * 12 * startVelocity + 20 * (targetValue - currentValue));
                a4 = 1 / (2 * d4) * (3 * startAcceleration * d2 + d1 * 16 * startVelocity + 30 * (currentValue - targetValue));
                a5 = 1 / (2 * d5) * (-1 * startAcceleration * d2 - d1 * 6 * startVelocity + 12 * (targetValue - currentValue));
            }

            public void ResetStartValues() {
                startVelocity = 0;
                startAcceleration = 0;
            }

            public double GetNextValue(double elapsedTime) {
                double t1 = elapsedTime;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                startVelocity = a1 + 2 * a2 * t1 + 3 * a3 * t2 + 4 * a4 * t3 + 5 * a5 * t4;
                startAcceleration = 2 * a2 + 6 * a3 * t1 + 12 * a4 * t2 + 20 * a5 * t3;

                return a0 + a1 * t1 + a2 * t2 + a3 * t3 + a4 * t4 + a5 * t5;
            }

            public double CalculateTime(double currentValue, double targetValue, double maxVelocity) {
                if (maxVelocity <= 0.0) {
                    throw new ArgumentException("velocity err");
                }

                double a = -1 / 32 * startAcceleration;
                double b = -7 / 16 * startVelocity - maxVelocity;
                double c = 15 / 8 * (targetValue - currentValue);

                double delta = b * b - 4 * a * c;

                if (delta < 0.0) {
                    throw new Exception("no real roots err");
                } else if (delta == 0.0) {
                    double T = -b / (2 * a);

                    if (T >= 0) {
                        return T;
                    } else {
                        throw new Exception("neg time err");
                    }
                } else {
                    double bSqrt = Math.Sqrt(b);
                    double T1 = (-b - bSqrt) / (2 * a);
                    double T2 = (-b + bSqrt) / (2 * a);
                    
                    if (T1 > 0.0 || T2 > 0.0) {
                        if (T1 < T2 && T1 > 0) {
                            return T1;
                        } else {
                            return T2;
                        }
                    } else {
                        throw new Exception("neg time err");
                    }
                }
            }

        }

        private readonly Polynominal polyX = new Polynominal();

        private readonly Polynominal polyY = new Polynominal();

        private readonly Polynominal polyZ = new Polynominal();

        private readonly Polynominal polyA = new Polynominal();

        private readonly Polynominal polyB = new Polynominal();

        private readonly Polynominal polyC = new Polynominal();

        private E6POS prevTargetPosition;

        private double prevMovementDuration;

        private double elapsedTime;

        public TrajectoryGenerator2(E6POS currentPosition) {
            prevTargetPosition = currentPosition;
        }

        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, double movementDuration) {
            if (currentPosition.Compare(targetPosition, 0.1, 0.1)) {
                elapsedTime = 0;
                ResetStartValues();
                return currentPosition;
            }

            if (targetPosition.Compare(prevTargetPosition, 0.1, 0.1) || movementDuration != prevMovementDuration) {
                UpdateCoefficients(currentPosition, targetPosition, movementDuration);
                elapsedTime = 0;
            } else {
                elapsedTime += 0.004;

                if (elapsedTime >= movementDuration && !currentPosition.Compare(targetPosition, 0.1, 0.1)) {
                    throw new Exception("too slow");
                }
            }

            prevTargetPosition = targetPosition;
            prevMovementDuration = movementDuration;

            E6POS nextPosition = new E6POS(
                polyX.GetNextValue(elapsedTime),
                polyY.GetNextValue(elapsedTime),
                polyZ.GetNextValue(elapsedTime),
                polyA.GetNextValue(elapsedTime),
                -polyB.GetNextValue(elapsedTime),
                -polyC.GetNextValue(elapsedTime)
            );

            return nextPosition - currentPosition;
        }

        private void UpdateCoefficients(E6POS currentPosition, E6POS targetPosition, double targetDuration) {
            polyX.UpdateCoefficients(currentPosition.X, targetPosition.X, targetDuration);
            polyY.UpdateCoefficients(currentPosition.Y, targetPosition.Y, targetDuration);
            polyZ.UpdateCoefficients(currentPosition.Z, targetPosition.Z, targetDuration);
            polyA.UpdateCoefficients(currentPosition.A, targetPosition.A, targetDuration);
            polyB.UpdateCoefficients(currentPosition.B, targetPosition.B, targetDuration);
            polyC.UpdateCoefficients(currentPosition.C, targetPosition.C, targetDuration);
        }

        private void ResetStartValues() {
            polyX.ResetStartValues();
            polyY.ResetStartValues();
            polyZ.ResetStartValues();
            polyA.ResetStartValues();
            polyB.ResetStartValues();
            polyC.ResetStartValues();
        }

    }
}
