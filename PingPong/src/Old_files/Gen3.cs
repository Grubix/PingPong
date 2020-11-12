using System;
using System.Threading;

namespace PingPong {

    class Gen3 {

        public class SCurve {

            private const double Ts = 0.004;

            private double t;

            private double x, x0, x1;

            public double vmin, vmax, v, v0 = 0;

            public double amin, amax, a, a0 = 0;

            public double jmin, jmax, j;

            public SCurve() {
                vmax = 0.5 / 0.004;
                vmin = -vmax;

                amax = 200;
                amin = -amax;

                jmax = 10000;
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
                double T3 = -amax / jmin;
                double V1 = v0 + a0 * T1 + jmax / 2.0 * T1 * T1;
                double T2 = 1.0 / amax * (vmax - V1 - jmin / 2.0 * T3 * T3 - amax * T3);
                double V2 = amax * T2 + V1;

                if (T2 < 0) {
                    Console.WriteLine("err1");
                }

                double T5 = amin / jmin;
                double T7 = -amin / jmax;
                double V3 = 1.0 / 2.0 * jmin * T5 * T5 + vmax;
                double T6 = -1.0 / amin * (V3 + jmax / 2.0 * T7 * T7 + amin * T7);
                double V4 = amin * T6 + V3;

                if (T6 < 0) {
                    Console.WriteLine("err2");
                }

                double T4 = 0.0;

                //Console.WriteLine($"{T1},  {T2},  {T3},  {T4},  {T5},  {T6},  {T7}");
                //Thread.Sleep(999999);

                t += Ts;

                return 5 * t;

                //return x;
            }

        }

        public SCurve X = new SCurve();

        public double GetNextValue(double currentValue, double targetValue, double velocity) {
            return X.GetNextValue(currentValue, targetValue, velocity);
        }

    }

}
