using System;

namespace PingPong {
    class Gen {

        public class SCurve {

            private const double Ts = 0.004;

            private double x, xPrev;

            public double V, vmin, vmax, v, vPrev;

            public double A, amin, amax, a, aPrev;

            public double J, jmin, jmax, j, jPrev;

            private int k, p;

            private bool decelerationPhase = false;

            private double T1, T2, Td;

            private double x0, x1;

            private double sign;

            public SCurve() {
                V = 125;
                A = 50;
                J = 100;
            }

            public double GetNextValue(double start, double end, double vel) {
                if (Math.Abs(start - end) <= 0.001) { //TODO: moze to wgl wywalic ??? wtedy ruch jest znacznie lepszy na koncowce
                    decelerationPhase = false;
                    v = a = j = vPrev = aPrev = jPrev = 0;
                    k = 0;
                    vPrev = 0.0;
                    return start;
                } else {
                    if (Math.Abs(end) != Math.Abs(x1)) {
                        sign = Math.Sign(end - start);

                        x0 = sign * start;
                        x1 = sign * end;

                        vmin = -sign * V;
                        vmax = sign * V;
                        amin = -sign * A;
                        amax = sign * A;
                        jmin = -sign * J;
                        jmax = sign * J;

                        decelerationPhase = false;
                    }
                }

                if (!decelerationPhase) {
                    T1 = (amin - aPrev) / jmin;
                    T2 = (0 - amin) / jmax; //TODO: tutaj a0 zamiast 0
                    Td = -vPrev / amin + T1 * (amin - aPrev) / (2.0 * amin) + T2 / 2.0;

                    if (Td < T1 + T2) {
                        double sqrt = Math.Sqrt((jmax - jmin) * (aPrev * aPrev * jmax - jmin * (2.0 * jmax * vPrev)));

                        T1 = -aPrev / jmin + 1.0 / (jmin * (jmin - jmax)) * sqrt;
                        T2 = 1.0 / (jmax * (jmax - jmin)) * sqrt;
                        Td = T1 + T2;
                    }

                    double h =
                        Td * vPrev + 
                        1.0 / 2.0 * a * Td * Td +
                        (jmin * T1 * (3.0 * Td * Td - 3.0 * Td * T1 + T1 * T1) + jmax * T2 * T2 * T2) / 6.0;

                   // Console.WriteLine($"{T1}, {T2}, {Td}, {h}, {x1 - x}");

                    if (h < x1 - x) {
                        if (vPrev - aPrev * aPrev / (2 * jmin) < vmax) {
                            if (aPrev < amax) {
                                Console.WriteLine("p1");
                                j = jmax;
                            } else {
                                Console.WriteLine("p2");
                                j = 0.0;
                            }
                        } else {
                            if (aPrev > 0) {
                                Console.WriteLine("p3");
                                j = jmin;
                            } else {
                                Console.WriteLine("p4");
                                j = 0.0;
                            }
                        }
                    } else {
                        p = k;
                        decelerationPhase = true;

                        int delta = k - p;

                        if (delta >= 0 && delta < T1 / Ts) {
                            j = jmin;
                        } else if (delta >= T1 / Ts && delta <= (Td - T2) / Ts) {
                            j = 0;
                        } else if (delta > (Td - T2) / Ts && delta <= Td / Ts) {
                            j = jmax;
                        } else {
                            return start;
                        }
                    }
                } else {
                    int delta = k - p;

                    if (delta >= 0 && delta < T1 / Ts) {
                        j = jmin;
                    } else if (delta >= T1 / Ts && delta <= (Td - T2) / Ts) {
                        j = 0;
                    } else if (delta > (Td - T2) / Ts && delta <= Td / Ts) {
                        j = jmax;
                    } else {
                        return start;
                    }
                }

                a = sign * (aPrev + Ts / 2.0 * (jPrev + j));
                v = sign * (vPrev + Ts / 2.0 * (aPrev + a));
                x = sign * (xPrev + Ts / 2.0 * (vPrev + v));

                jPrev = j;
                aPrev = a;
                vPrev = v;
                xPrev = x;

                Console.WriteLine(x);

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
