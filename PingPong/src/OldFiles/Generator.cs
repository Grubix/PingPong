using System;

namespace PingPong {
    class Generator {

        public class SCurve {

            public double v, v0, v1, va;

            private double time, T1, T2, T3, T12, T123;

            private double w1, w3;

            private double x, x0, x1, L;

            private double I1, I2, I3;

            public double a;

            private double A = 45.0;

            private double a0 = 0.0;


            public double GetNextValue(double currentValue, double targetValue, double maxVelocity) {
                if (Math.Abs(currentValue - targetValue) <= 0.001) {
                    v = a = 0;
                    return currentValue;
                }

                if (targetValue != x1 /*TODO: vel */) {
                    UpdateParameters(currentValue, targetValue, maxVelocity);
                    time = 0.0;
                }

                if (time >= 0.0 && time < T1) {
                    ComputePhase1();
                } else if (time >= T1 && time <= T12) {
                    ComputePhase2();
                } else if (time > T12 && time <= T123) {
                    ComputePhase3();
                }

                //Console.WriteLine(a);

                time += 0.004;

                return x;
            }

            private void UpdateParameters(double currentValue, double targetValue, double maxVelocity) {
                x0 = currentValue;
                x1 = targetValue;
                L = x1 - x0;

                v0 = v;
                v1 = (L < 0 ? -1.0 : 1.0) * maxVelocity;
                va = (v1 - v0) / 2.0;
                a0 = a;

                T1 = Math.PI * (v1 - v0) / (2.0 * (A - a0));
                T3 = Math.PI * v1 / (2.0 * A);

                I1 = T1 * (v0 + (v1 - v0) / 2.0);
                I3 = T3 * v1 / 2;

                if (Math.Abs(I1 + I3) > Math.Abs(L)) {
                    Console.WriteLine("asdasd");

                    double s = L < 0 ? -1.0 : 1.0;
                    v1 = s * FindNewVelocity();

                    Console.WriteLine(v1);

                    T1 = Math.PI * (v1 - v0) / (2.0 * (s * A - a0));
                    T3 = Math.PI * v1 / (2.0 * s * A);
                    T2 = 0.0;

                    va = (v1 - v0) / 2.0;

                    I1 = T1 * (v0 + (v1 - v0) / 2.0);
                    I2 = 0.0;
                    I3 = T3 * v1 / 2;

                    //Console.WriteLine(T1);
                    //Console.WriteLine(T3);
                    //Console.WriteLine(I1 + I3);

                } else {
                    I2 = x1 - x0 - I1 - I3;
                    T2 = Math.Abs(I2 / v1);
                }

                T12 = T1 + T2;
                T123 = T1 + T2 + T3;

                w1 = Math.PI / T1;
                w3 = Math.PI / T3;
            }

            private double FindNewVelocity() {
                double term = A * (4 * A * L - 4 * L * a0 + Math.PI * v0 * v0) / (Math.PI * (2 * A - a0));

                if (term < 0) {
                    double A2 = -A;
                    term = A2 * (4 * A2 * L - 4 * L * a0 + Math.PI * v0 * v0) / (Math.PI * (2 * A2 - a0));
                    Console.WriteLine("err");
                }

                return Math.Sqrt(term);
            }

            private void ComputePhase1() {
                x = x0 + v0 * time + va * (time - 1 / w1 * Math.Sin(w1 * time));
                v = v0 + a0 * time + va * (1 - Math.Cos(w1 * time));
                a = w1 * va * Math.Sin(w1 * time) + a0;
                //jerk = w1 * w1 * va * Math.Cos(w1 * time);
            }

            private void ComputePhase2() {
                double t2 = time - T1;

                x = x0 + I1 + v1 * t2;
                v = v1;
                a = 0.0;
                //jerk = 0.0;
            }

            private void ComputePhase3() {
                double t3 = time - (T1 + T2);

                x = x0 + I1 + I2 + v1 / 2.0 * (t3 + 1 / w3 * Math.Sin(w3 * t3));
                v = v1 / 2 * (1 + Math.Cos(w3 * t3));
                a = -w3 * v1 / 2.0 * Math.Sin(w3 * t3);
                //jerk = -w3 * w3 * v1 / 2.0 * Math.Cos(w3 * t3);
            }
        }

        public SCurve X = new SCurve();

        public double GetNextValue(double current, double target, double velocity) {
            return X.GetNextValue(current, target, velocity);
        }

    }
}
