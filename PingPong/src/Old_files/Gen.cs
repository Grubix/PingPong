using System;

namespace PingPong {
    class Gen {

        public class SCurve {

            private const double Ts = 0.004;

            private double x, x_1;

            public double vmin, vmax, v, v_1;

            public double amin, amax, a, a_1;

            public double jmin, jmax, j, j_1;

            private int k, p;

            private bool decelerationPhase = false;

            private double Td1, Td2, Td;

            private double oldX;

            public SCurve() {
                vmax = 125;
                vmin = -vmax;

                amax = 50;
                amin = -amax;

                jmax = 100;
                jmin = -jmax;
            }

            public double GetNextValue(double x0, double x1, double vel) {
                if (Math.Abs(x0 - x1) <= 0.001) { //TODO: moze to wgl wywalic ??? wtedy ruch jest znacznie lepszy na koncowce
                    decelerationPhase = false;
                    v = a = j = v_1 = a_1 = j_1 = 0;
                    k = 0;
                    v_1 = 0.0;
                    return x0;
                } else {
                    if (x1 != oldX) {
                        decelerationPhase = false;
                    }

                    oldX = x1;
                }

                if (!decelerationPhase) {
                    Td1 = (amin - a) / jmin;
                    Td2 = (0 - amin) / jmax; //TODO: tutaj a0
                    Td = -v_1 / amin + Td1 * (amin - a_1) / (2.0 * amin) + Td2 / 2.0;

                    if (Td < Td1 + Td2) {
                        Td1 = -a / jmin + 1.0 / (jmin * (jmin - jmax)) * Math.Sqrt((jmax - jmin) * (a * a * jmax - jmin * (2.0 * jmax * v_1)));
                        Td2 = 1.0 / (jmax * (jmax - jmin)) * Math.Sqrt((jmax - jmin) * (a * a * jmax - jmin * (2.0 * jmax * v_1)));
                        Td = Td1 + Td2;
                    }

                    double Td_2 = Td * Td;
                    double Tj2a_2 = Td1 * Td1;
                    double Tj2b_3 = Td2 * Td2 * Td2;

                    double h = 1.0 / 2.0 * a * Td_2 +
                        (jmin * Td1 * (3 * Td_2 - 3.0 * Td * Td1 + Tj2a_2) + jmax * Tj2b_3) / 6.0 +
                        Td * v_1;

                    if (h < x1 - x) {
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
                        p = k;
                        decelerationPhase = true;

                        int delta = k - p;

                        if (delta >= 0 && delta < Td1 / Ts) {
                            //Console.WriteLine("1");

                            j = jmin;
                        } else if (delta >= Td1 / Ts && delta <= (Td - Td2) / Ts) {
                            //Console.WriteLine($"{Td1 / Ts}[  {delta}  ]{(Td - Td2) / Ts}");
                            //Console.WriteLine("2");

                            j = 0;
                        } else if (delta > (Td - Td2) / Ts && delta <= Td / Ts) {
                            //Console.WriteLine("3");

                            j = jmax;
                        } else {
                            return x0;
                        }
                    }
                } else {
                    int delta = k - p;

                    if (delta >= 0 && delta < Td1 / Ts) {
                        //Console.WriteLine("1");

                        j = jmin;
                    } else if (delta >= Td1 / Ts && delta <= (Td - Td2) / Ts) {
                        //Console.WriteLine($"{Td1 / Ts}[  {delta}  ]{(Td - Td2) / Ts}");
                        //Console.WriteLine("2");

                        j = 0;
                    } else if (delta > (Td - Td2) / Ts && delta <= Td / Ts) {
                        //Console.WriteLine("3");

                        j = jmax;
                    } else {
                        return x0;
                    }
                }

                a = a_1 + Ts / 2.0 * (j_1 + j);
                v = v_1 + Ts / 2.0 * (a_1 + a);
                x = x_1 + Ts / 2.0 * (v_1 + v);

                Console.WriteLine(x - x_1);

                j_1 = j;
                a_1 = a;
                v_1 = v;
                x_1 = x;

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
