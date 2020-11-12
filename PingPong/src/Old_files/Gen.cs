using System;

namespace PingPong {
    class Gen {

        public class SCurve {

            private const double Ts = 0.004;

            private double x, x_1;

            public double vmin, vmax, v, v_1;

            public double amin, amax, a, a_1;

            public double jmin, jmax, j, j_1;

            private uint k, p;

            private bool decelerationPhase = false;

            public SCurve() {
                vmax = 0.5 / 0.004;
                vmin = -vmax;

                amax = vmax / 0.004;
                amin = -amax;

                jmax = amax / 0.004;
                jmin = -jmax;

                Console.WriteLine(vmax);
                Console.WriteLine(amax);
                Console.WriteLine(jmax);
            }

            public double GetNextValue(double x0, double x1, double vmax) {
                if (Math.Abs(x0 - x1) <= 0.01) {
                    decelerationPhase = false;
                    v = a = j = v_1 = a_1 = j_1 = 0;
                    k = 0;
                    return x0;
                }

                double Tj2a = (amin - a) / jmin; 
                double Tj2b = (a - amin) / jmax;
                double Td = -v / amin + Tj2a * (amin - a) / (2 * amin) + Tj2b / 2.0;
                double Td_2 = Td * Td;
                double Tj2a_2 = Tj2a * Tj2a;
                double Tj2b_3 = Tj2b * Tj2b * Tj2b;

                double h = 1.0 / 2.0 * a * Td_2 + 
                    (jmin * Tj2a * (3 * Td_2 - 3.0 * Td * Tj2a + Tj2a_2) + jmax * Tj2b_3) / 6.0 + 
                    Td * v;

                if (h < x1 - x0) {
                    // Phase 1 - acceleration

                    if (v - a * a / (2 * jmin) < vmax) {
                        if (a < amax) {
                            j = jmax;
                        } else {
                            j = 0.0;
                        }
                    } else {
                        if (a > 0) {
                            j = jmin;
                        } else {
                            j = 0.0;
                        }
                    }
                } else {
                    // Phase 2 - deceleration
                    decelerationPhase = true;
                    p = k;
                }

                if (decelerationPhase) {
                    uint delta = k - p;

                    if (delta >= 0 && delta < Tj2a / Ts) {
                        j = jmin;
                    } else if(delta >= Tj2a / Ts && delta <= (Td - Tj2b) / Ts) {
                        j = 0.0;
                    } else {
                        j = jmax;
                    }
                }

                a = a_1 + Ts / 2.0 * (j_1 + j);
                v = v_1 + Ts / 2.0 * (a_1 + a);
                x = x_1 + Ts / 2.0 * (v_1 + v);

                j_1 = j;
                a_1 = a;
                v_1 = v;
                x_1 = x;

                Console.WriteLine(v);

                k++;
                return x;
            }

        }

        public SCurve X = new SCurve();

        public double GetNextValue(double currentValue, double targetValue, double velocity) {
            return X.GetNextValue(currentValue, targetValue, velocity);
        }

    }
}
