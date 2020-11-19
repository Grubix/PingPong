using System;

namespace PingPong {
    class Gen5 {

        public class Polynominal {

            private double k0, k1, k2, k3;

            public double x0, v0, x1, vmax;

            public double x, v;

            public double t;

            public void UpdateCoefficients(double currentValue, double targetValue, double velocity) {
                t = 0.0;
                
                x0 = currentValue;
                x1 = targetValue;
                v0 = v;
                vmax = velocity;


                double T_1 = CalculateTime();
                double T_2 = T_1 * T_1;
                double T_3 = T_1 * T_2;

                k0 = x0;
                k1 = v0;
                k2 = (-2.0 * T_1 * v0 + 3.0 * (x1 - x0)) / T_2;
                k3 = (T_1 * v0 - 2.0 * (x1 - x0)) / T_3;
            }

            public double GetNextValue(double currentValue, double targetValue, double velocity) {
                if (Math.Abs(currentValue - targetValue) <= 0.01) {
                    v0 = 0.0;
                    return currentValue;
                }

                if (targetValue != x1 || vmax != velocity) {
                    UpdateCoefficients(currentValue, targetValue, velocity);
                }

                x = k3 * t * t * t + k2 * t * t + k1 * t + k0;
                v = 3.0 * k3 * t * t + 2.0 * k2 * t + k1;

                t += 0.004;

                return x;
            }

            private double CalculateTime() {
                double a = -v0 * (v0 + vmax);
                double b = 6 * (x1 - x0) * (vmax + v0);
                double c = -9 * (x0 - x1) * (x0 - x1);

                double[] roots = QuadraticSolver.SolveReal(a, b, c);

                if (roots.Length == 0) {
                    throw new Exception("adasd");
                }

                foreach (var item in roots) {
                    Console.WriteLine(item);
                }

                return 5.0;
            }

        }

        public readonly Polynominal X = new Polynominal();

        public double GetNextValue(double currentPosition, double targetPosition, double velocity) {
            return X.GetNextValue(currentPosition, targetPosition, velocity);
        }

    }
}
