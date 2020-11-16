using System;

namespace PingPong {

    class Gen4 {

        public class SCurve {

            private const double Ts = 0.004;

            private double t;

            private double x, x0, x1;

            public double V, vmin, vmax, v, v0;

            public double A, amin, amax, alim, a, a0;

            public double J, jmin, jmax, j;

            private double T1, T2, T3, T4, T5, T6, T7;

            private double V1, V2, V3, V4;

            private double X1, X2, X3, X4, X5, X6;

            private bool invert = false;

            public SCurve() {
                A = 1;
                J = 1;
            }

            public double GetNextValue(double start, double end, double maxVelocity) {
                if (Math.Abs(start - end) <= 0.00001) {
                    t = 0.0;
                    a = 0;
                    v = 0;
                    return start;
                }

                if (Math.Abs(end) != Math.Abs(x1) || Math.Abs(maxVelocity) != Math.Abs(V)) {
                    t = 0.0;

                    x0 = start;
                    x1 = end;

                    v0 = v;
                    a0 = a;

                    V = maxVelocity;
                    vmax = V;
                    vmin = -V;

                    amax = A;
                    amin = -A;

                    jmax = J;
                    jmin = -J;

                    T1 = (amax - a0) / jmax;
                    T3 = -amax / jmin;
                    V1 = v0 + a0 * T1 + jmax / 2.0 * T1 * T1;
                    T2 = 1.0 / amax * (vmax - V1 - jmin / 2.0 * T3 * T3 - amax * T3);
                    V2 = amax * T2 + V1;

                    if (T2 < 0) {
                        Console.WriteLine("T2");

                        // Gwarancja T2 = 0 i osiągnięcia vmax po czasie T1 + T3
                        amax = Math.Sqrt((2 * jmin * jmax * (vmax - v0) + jmin * a0 * a0) / (jmin - jmax));

                        T1 = (amax - a0) / J;
                        T2 = 0.0;
                        T3 = amax / J;

                        V1 = v0 + a0 * T1 + J / 2.0 * T1 * T1;
                        V2 = amax * T2 + V1;
                    }

                    T5 = amin / jmin;
                    T7 = -amin / jmax;
                    V3 = 1.0 / 2.0 * jmin * T5 * T5 + vmax;
                    T6 = -1.0 / amin * (V3 + jmax / 2.0 * T7 * T7 + amin * T7);
                    V4 = amin * T6 + V3;

                    if (T6 < 0) {
                        Console.WriteLine("T6");

                        // Gwarancja T6 = 0 i osiągnięcia v = 0 po czasie T5 + T7 przy początkowej prędkości v = vmax
                        amin = Math.Sqrt(2.0 * jmin * jmax * vmax / (jmax - jmin));

                        T5 = amin / jmin;
                        T6 = 0.0;
                        T7 = -amin / jmax;

                        V3 = 1.0 / 2.0 * jmin * T5 * T5 + vmax;
                        V4 = amin * T6 + V3;
                    }

                    X1 = jmax / 6.0 * T1 * T1 * T1 + a0 / 2.0 * T1 * T1 + v0 * T1 + x0;
                    X2 = amax / 2.0 * T2 * T2 + V1 * T2 + X1;
                    X3 = jmin / 6.0 * T3 * T3 * T3 + amax / 2.0 * T3 * T3 + V2 * T3 + X2;

                    T4 = -1.0 / vmax * (T7 * T7 * T7 * jmax / 6.0 + T5 * T5 * T5 * jmin / 6.0 + (T7 * T7 + T6 * T6) / 2.0 * amin + V4 * T7 + V3 * T6 + T5 * vmax - x1 + X3); //x0 jest juz w X3 (X1) !

                    if (T4 < 0) {
                        Console.WriteLine("T4");
                        double a = -jmax * jmax;
                        double b = -jmax * (amax * amax + jmax * v0);
                        double c = amax * jmax * jmax * (x1 - x0) + jmax * amax * v0 * a0 - amax * amax * jmax * v0 - a0 * a0 * jmax * v0/2.0 - amax * a0 * a0 * a0 / 3.0 + amax * amax * a0 * a0 / 4.0 + a0 * a0 * a0 * a0 / 8.0;

                        double[] roots = QuadraticSolver.SolveReal(a, b, c);
                        if (roots[0] < 0) {
                            vmax = roots[1];
                        } else {
                            vmax = roots[0];
                        }

                        Console.WriteLine("v=" + vmax);

                        //Console.WriteLine($"{roots[0]},{roots[1]}:::{vmax}");

                        //A = 1, J = 1, disp = 12 => vmax = 3
                        //vmax = 1;

                        //    //if (roots.Length == 0) {
                        //    //    throw new Exception("adasdasd");
                        //    //}

                        //    //vmax = -1.0 / (6.0 * amin * jmax) * (3 * jmax * jmax * x1 - 3 * amin * amin * amin - 3 * amax * amax * amax - 6 * amax * jmax * v0 - a0 * a0 * a0 + 3 * a0 * a0 * amax + 3 * jmax * a0 * v0 - 3 * jmax * jmax * x0);
                        //    //Console.WriteLine(vmax);

                        //T1 = (amax - a0) / jmax;
                        //T2 = 1.0 / amax * (vmax + amax * amax * (jmax - jmin) / (2 * jmin * jmax) + a0 * a0 / (2 * jmax));
                        //T3 = -amax / jmin;

                        //T5 = amin / jmin;
                        //T6 = -1.0 / amin * (vmax + amin * amin * (jmax - jmin) / (2 * jmin * jmax));
                        //T7 = -amin / jmax;

                        //V1 = v0 + a0 * T1 + jmax / 2.0 * T1 * T1;
                        //V2 = amax * T2 + V1;
                        //V3 = 1.0 / 2.0 * jmin * T5 * T5 + vmax;
                        //V4 = amin * T6 + V3;

                        T1 = (amax - a0) / jmax;
                        T3 = -amax / jmin;
                        V1 = v0 + a0 * T1 + jmax / 2.0 * T1 * T1;
                        T2 = 1.0 / amax * (vmax - V1 - jmin / 2.0 * T3 * T3 - amax * T3);
                        V2 = amax * T2 + V1;

                        if (T2 < 0) {
                            Console.WriteLine("T2");

                            // Gwarancja T2 = 0 i osiągnięcia vmax po czasie T1 + T3
                            amax = Math.Sqrt(Math.Abs((2 * jmin * jmax * (vmax - v0) + jmin * a0 * a0) / (jmin - jmax)));

                            T1 = (amax - a0) / J;
                            T2 = 0.0;
                            T3 = amax / J;

                            V1 = v0 + a0 * T1 + J / 2.0 * T1 * T1;
                            V2 = amax * T2 + V1;
                        }

                        T5 = amin / jmin;
                        T7 = -amin / jmax;
                        V3 = 1.0 / 2.0 * jmin * T5 * T5 + vmax;
                        T6 = -1.0 / amin * (V3 + jmax / 2.0 * T7 * T7 + amin * T7);
                        V4 = amin * T6 + V3;

                        X1 = jmax / 6.0 * T1 * T1 * T1 + a0 / 2.0 * T1 * T1 + v0 * T1 + x0;
                        X2 = amax / 2.0 * T2 * T2 + V1 * T2 + X1;
                        X3 = jmin / 6.0 * T3 * T3 * T3 + amax / 2.0 * T3 * T3 + V2 * T3 + X2;

                        T4 = -1.0 / vmax * (T7 * T7 * T7 * jmax / 6.0 + T5 * T5 * T5 * jmin / 6.0 + (T7 * T7 + T6 * T6) / 2.0 * amin + V4 * T7 + V3 * T6 + T5 * vmax - x1 + X3); //x0 jest juz w X3 (X1) !
                        Console.WriteLine($"{T1}, {T2}, {T3}, {T4}, {T5}, {T6}, {T7}");

                    }

                    X4 = vmax * T4 + X3;
                    X5 = jmin / 6.0 * T5 * T5 * T5 + vmax * T5 + X4;
                    X6 = amin / 2.0 * T6 * T6 + V3 * T6 + X5;
                    double X7 = jmax / 6.0 * T7 * T7 * T7 + amin / 2.0 * T7 * T7 + V4 * T7 + X6;

                    Console.WriteLine(X7);

                    //Console.WriteLine($"{T1}, {T2}, {T3}, {T4}, {T5}, {T6}, {T7}");
                }

                if (t < T1) {

                    x = jmax / 6.0 * t * t * t + a0 / 2.0 * t * t + v0 * t + x0;
                    v = jmax / 2.0 * t * t + a0 * t + v0;
                    a = jmax * t + a0;

                } else if (t < T1 + T2) {
                    double ts = t - T1;

                    x = amax / 2.0 * ts * ts + V1 * ts + X1;
                    v = amax * ts + V1;
                    a = amax;
                } else if (t < T1 + T2 + T3) {
                    double ts = t - (T1 + T2);

                    x = jmin / 6.0 * ts * ts * ts + amax / 2.0 * ts * ts + V2 * ts + X2;
                    v = jmin / 2.0 * ts * ts + amax * ts + V2;
                    a = jmin * ts + amax;

                } else if (t < T1 + T2 + T3 + T4) {
                    double ts = t - (T1 + T2 + T3);

                    x = vmax * ts + X3;
                    v = vmax;
                    a = 0.0;
                } else if (t < T1 + T2 + T3 + T4 + T5) {
                    double ts = t - (T1 + T2 + T3 + T4);

                    x = jmin / 6.0 * ts * ts * ts + vmax * ts + X4;
                    v = jmin / 2.0 * ts * ts + vmax;
                    a = jmin * ts;
                } else if (t < T1 + T2 + T3 + T4 + T5 + T6) {
                    double ts = t - (T1 + T2 + T3 + T4 + T5);

                    x = amin / 2.0 * ts * ts + V3 * ts + X5;
                    v = amin * ts + V3;
                    a = amin;
                } else if (t < T1 + T2 + T3 + T4 + T5 + T6 + T7) {
                    double ts = t - (T1 + T2 + T3 + T4 + T5 + T6);

                    x = jmax / 6.0 * ts * ts * ts + amin / 2.0 * ts * ts + V4 * ts + X6;
                    v = jmax / 2.0 * ts * ts + amin * ts + V4;
                    a = jmax * ts + amin;
                }

                //if (invert) {
                //    x = -x;
                //    v = -v;
                //    a = -a;
                //}

                //Thread.Sleep(999999);

                //Console.WriteLine(x);
                t += Ts;

                return x;
            }

            private void Invert() {
                //x0 = -x0;
                //x1 = -x1;
                //v0 = -v0;
                //a0 = -a0;

                vmin = -vmin;
                vmax = -vmax;
                amin = -amin;
                amax = -amax;
                jmin = -jmin;
                jmax = -jmax;
            }

        }

        public SCurve X = new SCurve();

        public double GetNextValue(double currentValue, double targetValue, double velocity) {
            return X.GetNextValue(currentValue, targetValue, velocity);
        }

    }

}
