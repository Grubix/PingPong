using PingPong;
using System;

namespace PingPongs {
    class TrajectoryGenerator7 {

        public class SCurve {

            private double i0, i1, i2, i3; // Phase 1 polynominal coefficients

            private double j0, j1, j2, j3; // Phase 3 polynominal coefficients

            private double k0, k1, k2, k3, k4, k5; // Phase 1, 3 polynominal coefficients (T2 = 0);

            private double T2, T12, T123;

            private double T1;

            private double T3;

            private double T4;

            private double x, x0, x1; // Position (current, start, target)

            public double v, v0, v1; // Velocity (current, start, target)

            public double a, a0 = 24.0, A = 25.0; // Acceleration (current, start, max)

            private double j; // Current jerk

            private double t; // Elapsed time (from last coefficients update)

            private double I1, I2;


            public double GetNextValue(double currentValue, double targetValue, double velocity) {
                if (velocity == v0 || velocity == 0) {
                    throw new ArgumentException("vel err");
                }

                if (Math.Abs(currentValue - targetValue) <= 0.01) {
                    v0 = a0 = 0.0;
                    return currentValue;
                }

                if ((targetValue != x1 || velocity != v1) && velocity != v0) {
                    UpdateCoefficients(currentValue, targetValue, velocity);
                }

                if (t < T1) {
                    double t1_1 = t;
                    double t1_2 = t1_1 * t1_1;
                    double t1_3 = t1_1 * t1_2;
                    double t1_4 = t1_1 * t1_3;

                    x = 1.0 / 4.0 * i3 * t1_4 + 1.0 / 3.0 * i2 * t1_3 + 1.0 / 2.0 * i1 * t1_2 + i0 * t1_1 + x0;
                    v = i2 * t1_3 + i2 * t1_2 + i1 * t1_1 + i0;
                } else if (t <= T12) {
                    double t2_1 = t - T1;

                    x = I1 + v1 * t2_1;
                    v = v1;
                } else if (t <= T123) {
                    double t3_1 = t - T12;
                    double t3_2 = t3_1 * t3_1;
                    double t3_3 = t3_1 * t3_2;
                    double t3_4 = t3_1 * t3_3;

                    x = 1.0 / 4.0 * j3 * t3_4 + 1.0 / 3.0 * j2 * t3_3 + 1.0 / 2.0 * j1 * t3_2 + j0 * t3_1 + x0 + I1 + I2;
                    v = j2 * t3_3 + j2 * t3_2 + j1 * t3_1 + j0;
                }

                t += 0.004;

                return x;
            }

            private void UpdateCoefficients(double currentValue, double targetValue, double velocity) {
                t = 0.0;
                x0 = currentValue;
                x1 = targetValue;
                v1 = velocity;
                v0 = v;
                //a0 = a;

                T1 = FindT1();
                T3 = FindT3();

                I1 = a0 / 12.0 * T1 * T1 + (v1 + v0) / 2.0 * T1;
                double I3 = v1 * T3 / 2.0;
                double L = x1 - x0;

                if (Math.Abs(I1 + I3) <= Math.Abs(L)) {
                    T2 = (L - I1 - I3) / v1;
                    I2 = T2 * v1;

                    i0 = v0;
                    i1 = a0;
                    i2 = 1.0 / (T1 * T1) * (-2 * T1 * a0 + 3 * (v1 - v0));
                    i3 = 1.0 / (T1 * T1 * T1) * (T1 * a0 + 2 * (v0 - v1));

                    j0 = v1;
                    j1 = 0.0;
                    j2 = -3.0 * v1 / (T3 * T3);
                    j3 = 2.0 * v1 / (T3 * T3 * T3);
                } else {
                    //TODO: inny wielomian
                }

                T12 = T1 + T2;
                T123 = T1 + T2 + T3;
            }

            private double FindT1() {
                double[] roots = QuadraticSolver.SolveReal(
                    -(3 * A * a0 - a0 * a0),
                    6 * (v1 - v0) * (A + a0),
                    -9 * (v1 - v0) * (v1 - v0)
                );

                if (roots.Length == 0) {
                    throw new InvalidOperationException("T1 err");
                } else {
                    if (roots[0] <= 0) {
                        Console.WriteLine(roots[0]);
                        Console.WriteLine(roots[1]);
                        throw new InvalidOperationException("T1 err");
                    } else {
                        return roots[0];
                    }
                }
            }

            private double FindT3() {
                double T3 = -3.0 * -v1 / (2.0 * (A - a0)); //TODO - przed v1 !!!

                if (T3 <= 0) {
                    throw new InvalidOperationException("T3 err");
                }

                return T3;
            }

        }

        public SCurve X = new SCurve();

        public double GetNextValue(double currentValue, double targetValue, double velocity) {
            return X.GetNextValue(currentValue, targetValue, velocity);
        }
    }
}
