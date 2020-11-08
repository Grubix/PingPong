using System;

namespace PingPong {
    class TrajectoryGenerator4 {

        public class SCurve {

            private const double PI = Math.PI;

            private double t, t1, t2, t3, t12, t123;

            private double x0, x1;

            public double v, v0, v1;

            private double h;

            double phase1Integral;

            double phase2Integral;

            double phase3Integral;

            public double GetNextValue(double currentValue, double targetValue, double velocity) {
                if (targetValue != x1 || velocity != v1) {
                    x1 = targetValue;
                    v1 = velocity;

                    double k = 1;
                    double p = 1;

                    t = 0.0;
                    h = Math.Sign(v1 - v0) * Math.Abs(v1 - v0);
                    t2 =  (targetValue - currentValue) / (k * (v0 + h / 2) + v1 * (1 + p / 2));
                    t1 = k * t2;
                    t3 = p * t2;
                    t12 = t1 + t2;
                    t123 = t1 + t2 + t3;

                    phase1Integral = t1 * (v0 + h / 2);
                    phase2Integral = v1 * t2;
                    phase3Integral = v1 * t3 / 2;
                }

                if (t >= 0.0 && t < t1) {
                    double sin = Math.Sin(t * PI / t1);

                    v = v0 + h / 2 * (1 - Math.Cos(PI / t1 * t));
                    x0 = 1 / (2.0 * PI) * (t * (2.0 * PI * v0 + h * PI) - h * t1 * sin);
                    t += 0.004;

                } else if (t >= t1 && t <= t12) {

                    v = v1;
                    x0 = phase1Integral + (t - t1) * v1;
                    t += 0.004;

                } else if (t > t12 && t <= t123) {
                    double sin1 = Math.Sin(PI * (t - t12) / t3);
                    double sin2 = Math.Sin(PI * t12 / t3);

                    v = v1 / 2 * (1 + Math.Cos(PI / t3 * (t - (t12))));
                    x0 = phase1Integral + phase2Integral + v1 / (2.0 * PI) * (PI * t + t3 * (sin1 - sin2));
                    t += 0.004;

                }

                return x0;
            }
        }

        public SCurve X = new SCurve();

        public double GetValue(double current, double target, double velocity) {
            return X.GetNextValue(current, target, velocity);
        }

    }
}
