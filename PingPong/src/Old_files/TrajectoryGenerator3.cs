using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator3 {

        public class Polynominal {

            private double k0, k1, k2, k3, k4, k5;

            private double prevTargetValue;

            private double prevMaxVelocity;

            private double duration;

            private double elapsedTime;

            public double CurrentVelocity { get; private set; }

            public double CurrentAcceleration { get; private set; }

            public Polynominal(double currentValue) {
                prevTargetValue = currentValue;
            }

            public double GetNextValue(double currentValue, double targetValue, double maxVelocity, double tolerance) {
                if (Math.Abs(currentValue - targetValue) <= tolerance) {
                    CurrentVelocity = 0.0;
                    CurrentAcceleration = 0.0;
                    return currentValue;
                }

                bool targetValueChanged = Math.Abs(targetValue - prevTargetValue) >= 1E-9;
                bool maxVelocityChanged = Math.Abs(maxVelocity - prevMaxVelocity) >= 1E-9;

                if ((targetValueChanged || maxVelocityChanged)) {
                    UpdateCoefficients(currentValue, targetValue, maxVelocity);
                    prevTargetValue = targetValue;
                    prevMaxVelocity = maxVelocity;
                    elapsedTime = 0.0;
                } else {
                    //elapsedTime += 0.004;
                }

                elapsedTime += 0.004;

                double t1 = elapsedTime;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                if(elapsedTime >= duration / 2) {
                    Console.WriteLine(CurrentVelocity);
                }

                CurrentVelocity = k1 + 2.0 * k2 * t1 + 3.0 * k3 * t2 + 4.0 * k4 * t3 + 5.0 * k5 * t4;
                CurrentAcceleration = 2.0 * k2 + 6.0 * k3 * t1 + 12.0 * k4 * t2 + 20.0 * k5 * t3;

                return k0 + k1 * t1 + k2 * t2 + k3 * t3 + k4 * t4 + k5 * t5;
            }

            private void UpdateCoefficients(double currentValue, double targetValue, double maxVelocity) {
                duration = CalculateDuration(currentValue, targetValue, maxVelocity);

                double d1 = duration;
                double d2 = d1 * d1;
                double d3 = d1 * d2;
                double d4 = d1 * d3;
                double d5 = d1 * d4;

                k0 = currentValue;
                k1 = CurrentVelocity;
                k2 = CurrentAcceleration / 2;
                k3 = 1.0 / (2.0 * d3) * (-3.0 * CurrentAcceleration * d2 - d1 * 12.0 * CurrentVelocity + 20.0 * (targetValue - currentValue));
                k4 = 1.0 / (2.0 * d4) * (3.0 * CurrentAcceleration * d2 + d1 * 16.0 * CurrentVelocity + 30.0 * (currentValue - targetValue));
                k5 = 1.0 / (2.0 * d5) * (-1.0 * CurrentAcceleration * d2 - d1 * 6.0 * CurrentVelocity + 12.0 * (targetValue - currentValue));
            }

            private double CalculateDuration(double currentValue, double targetValue, double maxVelocity) {
                if (maxVelocity <= 0.0) {
                    throw new ArgumentException("velocity err");
                }

                double a = -1.0 / 32.0 * Math.Abs(CurrentAcceleration);
                double b = -7.0 / 16.0 * Math.Abs(CurrentVelocity) - maxVelocity;
                double c = 15.0 / 8.0 * Math.Abs(targetValue - currentValue);
                double delta = b * b - 4.0 * a * c;

                if (a == 0.0) {
                    double T = -c / b;

                    if (T > 0.0) {
                        Console.WriteLine($"a={a}, b={b}, c={c}, delta={delta}, T={T}");
                        return T;
                    } else {
                        throw new ArgumentException("neg time err");
                    }
                }

                if (delta < 0.0) {
                    throw new ArgumentException("no real roots err");
                } else if (delta == 0.0) {
                    double T = -b / (2.0 * a);

                    if (T > 0.0) {
                        Console.WriteLine($"a={a}, b={b}, c={c}, delta={delta}, T={T}");
                        return T;
                    } else {
                        throw new ArgumentException("neg time err");
                    }
                } else {
                    double deltaSqrt = Math.Sqrt(delta);
                    double T1 = (-b - deltaSqrt) / (2.0 * a);
                    double T2 = (-b + deltaSqrt) / (2.0 * a);

                    if (T1 > 0.0 || T2 > 0.0) {
                        if (T1 > 0.0 && T2 > 0.0) {
                            Console.WriteLine($"a={a}, b={b}, c={c}, delta={delta}, T1={T1}, T2={T2}");
                            return Math.Min(T1, T2);
                        } else if (T1 > 0.0) {
                            Console.WriteLine($"a={a}, b={b}, c={c}, delta={delta}, T1={T1}, T2={T2}");
                            return T1;
                        } else {
                            Console.WriteLine($"a={a}, b={b}, c={c}, delta={delta}, T1={T1}, T2={T2}");
                            return T2;
                        }
                    } else {
                        throw new ArgumentException("neg time err");
                    }
                }
            }

        }

        public readonly Polynominal X;

        private readonly Polynominal Y;

        private readonly Polynominal Z;

        private readonly Polynominal A;

        private readonly Polynominal B;

        private readonly Polynominal C;

        public TrajectoryGenerator3(E6POS currentPosition) {
            X = new Polynominal(currentPosition.X);
            Y = new Polynominal(currentPosition.Y);
            Z = new Polynominal(currentPosition.Z);
            A = new Polynominal(currentPosition.A);
            B = new Polynominal(currentPosition.B);
            C = new Polynominal(currentPosition.C);
        }

        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, Vector<double> velocityXYZ, Vector<double> velocityABC) {
            E6POS nextPosition = new E6POS(
                X.GetNextValue(currentPosition.X, targetPosition.X, velocityXYZ[0], 0.01),
                Y.GetNextValue(currentPosition.Y, targetPosition.Y, velocityXYZ[1], 0.01),
                Z.GetNextValue(currentPosition.Z, targetPosition.Z, velocityXYZ[2], 0.01),
                A.GetNextValue(currentPosition.A, targetPosition.A, velocityABC[0], 0.1),
                -B.GetNextValue(currentPosition.B, targetPosition.B, velocityABC[1], 0.1),
                -C.GetNextValue(currentPosition.C, targetPosition.C, velocityABC[2], 0.1)
            );

            return nextPosition - currentPosition;
        }

        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, double velocityXYZ, double velocityABC) {
            var XYZ = Vector<double>.Build.DenseOfArray(new double[] { 
                velocityXYZ, 
                velocityXYZ, 
                velocityXYZ 
            });

            var ABC = Vector<double>.Build.DenseOfArray(new double[] { 
                velocityABC, 
                velocityABC, 
                velocityABC 
            });

            return GetNextCorrection(currentPosition, targetPosition, XYZ, ABC);
        }

    }
}
