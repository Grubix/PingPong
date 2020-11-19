using System;

namespace PingPong {

    class Gen2 {

        public class SCurve {

            private const double Ts = 0.004;

            private double t;

            private double x, x0, x1;

            public double vmin, vmax, v, v0 = 0;

            public double amin, amax, a, a0 = 5;

            public double jmin, jmax, j;

            public SCurve() {
                vmax = 0.5 / 0.004;
                vmin = -vmax;

                amax = 1000;
                amin = -amax;

                jmax = 5000;
                jmin = -jmax;
            }

            public double GetNextValue(double start, double end, double maxVelocity) {
                if (Math.Abs(start - end) <= 0.01) {
                    t = 0.0;
                    a = 0;
                    v = 0;
                    return start;
                }

                x1 = end;

                double T1 = (amax - a0) / jmax;
                double T2 = -amax / jmin;
                double Ta = 1.0 / amax * (vmax - v0 + 1.0 / 2.0 * (amax * (T1 + T2) - a0 * T1));

                double T3 = amin / jmin;
                double T4 = -amin / jmax;
                double Td = 1.0 / amin * (-vmax + 1.0 / 2.0 * amin * (T3 + T4));

                double Tv = (x1 - x0) / vmax - Ta / 2.0 * (1 + v0 / vmax) - Td / 2.0;
                double T = Ta + Tv + Td;

                if (Tv < 0) {
                    Console.WriteLine("err");
                }

                if (t < T1) {
                    // Segment 1
                    j = jmax;
                    a = a0 + jmax * t;
                    v = v0 + a0 * t + jmax / 2.0 * t * t;
                    x = x0 + v0 * t + jmax / 6.0 * t * t * t + a0 / 2.0 * t * t;
                } else if (t < Ta - T2) {
                    // Segment 2
                    j = 0.0;
                    a = a0 + jmax * T1;
                    v = v0 + a0 * t + jmax * T1 * (t - T1 / 2.0);
                    x = x0 + v0 * t + a0 * t * t / 2.0 + jmax * T1 / 6.0 * (3 * t * t - 3 * T1 * t + T1 * T1);
                } else if (t < Ta) {
                    // Segment 3
                    j = jmin;
                    a = -jmin * (Ta - t);
                    v = vmax + jmin * Math.Pow(Ta - t, 2.0) / 2.0;
                    x = x0 + (vmax + v0) * Ta / 2 - vmax * (Ta - t) - jmin / 6.0 * Math.Pow(Ta - t, 3.0);
                } else if (t < Ta + Tv) {
                    // Segment 4
                    j = 0.0;
                    a = 0.0;
                    v = vmax;
                    x = x0 + (vmax + v0) * Ta / 2 - vmax * (Ta - t);
                } else if (t < T - Td + T3) {
                    // Segment 5
                    j = jmin;
                    a = jmin * (t - T + Td);
                    v = vmax + jmin / 2.0 * Math.Pow(t - T + Td, 2.0);
                    x = x1 - vmax * Td / 2 + vmax * (t - T + Td) - jmax / 6.0 * Math.Pow(t - T + Td, 3.0);
                } else if (t < T - T4) {
                    // Segment 6
                    j = 0.0;
                    a = jmin * T3;
                    v = vmax + jmin * T3 * (t - T + Td - T3 / 2.0);
                    x = x0 - jmax / 6.0 * Math.Pow(T - t, 3.0);
                } else if (t < T) {
                    // Segment 7
                    j = 0.0;
                    a = -jmax * (T - t);
                    v = jmax / 2.0 * Math.Pow(T - t, 2.0);
                    x = x1 - jmax / 6.0 * Math.Pow(T - t, 3.0);
                }

                Console.WriteLine(x - start);

                t += Ts;

                return x;
            }

        }

        public SCurve X = new SCurve();

        public double GetNextValue(double currentValue, double targetValue, double velocity) {
            return X.GetNextValue(currentValue, targetValue, velocity);
        }

    }

}
