using PingPong.Maths;
using System;
using System.Windows.Forms;

namespace PingPong {
    class TrajectoryGenerator5 {

        public class SCurve {

            public double jerk, maxJerk = 5.0; // Max jerk

            public double velocity, v0, v1, va;

            private double time, T1, T2, T3, T12, T123;

            private double w1, w3;

            private double x, x0, x1, L;

            private double I1, I2, I3;

            public double acceleration;

            public double GetNextValue(double currentValue, double targetValue, double maxVelocity) {
                if (Math.Abs(currentValue - targetValue) <= 0.001) {
                    velocity = acceleration = jerk = 0.0;
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

                time += 0.004;

                return x;
            }

            private void UpdateParameters(double currentValue, double targetValue, double maxVelocity) {
                x0 = currentValue;
                x1 = targetValue;
                L = x1 - x0;

                v0 = velocity;
                v1 = (L < 0 ? -1.0 : 1.0) * maxVelocity;
                va = (v1 - v0) / 2.0;

                T1 = Math.PI * Math.Sqrt(Math.Abs(v1 - v0) / (2.0 * maxJerk)); // Jerk(0) = J
                T3 = Math.PI * Math.Sqrt(Math.Abs(v1) / (2.0 * maxJerk)); // Jerk(T1 + T2 + T3) = J

                I1 = T1 * (v0 + va);
                I3 = T3 * v1 / 2;

                if (Math.Abs(I1 + I3) > Math.Abs(L)) {
                    v1 = findNewVelocity();
                    va = (v1 - v0) / 2.0;
                    T1 = Math.PI * Math.Sqrt(Math.Abs(v1 - v0) / (2.0 * maxJerk));
                    T3 = Math.PI * Math.Sqrt(Math.Abs(v1) / (2.0 * maxJerk));

                    I1 = T1 * (v0 + va);
                    I3 = T3 * v1 / 2;
                    I2 = 0.0;
                    T2 = 0.0;

                    //Console.WriteLine(v1);
                    //Console.WriteLine(I1);
                    //Console.WriteLine(I3);
                    //Console.WriteLine(I1 + I3);
                } else {
                    I2 = x1 - x0 - I1 - I3;
                    T2 = Math.Abs(I2 / v1);
                }

                T12 = T1 + T2;
                T123 = T1 + T2 + T3;

                w1 = Math.PI / T1;
                w3 = Math.PI / T3;

                Console.WriteLine(T123);
            }

            private void ComputePhase1() {
                // t ∈ <0, T1)
                x = x0 + v0 * time + va * (time - 1 / w1 * Math.Sin(w1 * time));
                velocity = v0 + va * (1 - Math.Cos(w1 * time));
                acceleration = w1 * va * Math.Sin(w1 * time);
                jerk = w1 * w1 * va * Math.Cos(w1 * time);
            }

            private void ComputePhase2() {
                // t ∈ <T1, T1+T2>
                double t2 = time - T1;

                x = x0 + I1 + v1 * t2;
                velocity = v1;
                acceleration = 0.0;
                jerk = 0.0;
            }

            private void ComputePhase3() {
                // t ∈ (T1+T2, T1+T2+T3>
                double t3 = time - (T1 + T2);

                x = x0 + I1 + I2 + v1 / 2.0 * (t3 + 1 / w3 * Math.Sin(w3 * t3));
                velocity = v1 / 2 * (1 + Math.Cos(w3 * t3));
                acceleration = -w3 * v1 / 2.0 * Math.Sin(w3 * t3);
                jerk = -w3 * w3 * v1 / 2.0 * Math.Cos(w3 * t3);
            }

            private double findNewVelocity() {
                if (v0 == 0.0) {

                    return (L < 0 ? -1.0 : 1.0) * Math.Pow(2 * maxJerk * L * L / (Math.PI * Math.PI), 1.0 / 3.0);
                } else {
                    double pi2 = Math.PI * Math.PI;
                    double pi4 = pi2 * pi2;
                    double v02 = v0 * v0;
                    double v03 = v0 * v02;
                    double v04 = v0 * v03;
                    double v05 = v0 * v04;
                    double v06 = v0 * v05;

                    double a = v02 * pi4;
                    double b = -(2 * v03 * pi4 + 32 * maxJerk * L * pi2);
                    double c = -(v04 * pi4 + 16 * v0 * maxJerk * L * pi2);
                    double d = 2 * v05 * pi4 + 16 * v02 * maxJerk * L * pi2;
                    double e = v06 * pi4 + 16 * v03 * maxJerk * L * pi2 + 64 * maxJerk * maxJerk * L * L;

                    CubicSolver solver = new CubicSolver();
                    var roots = solver.Solve(a, b, c, d);

                    //TODO: sortowanie

                    return roots[0].re;
                }
            }
        }

        public SCurve X = new SCurve();

        public TrajectoryGenerator5() {
            
        }

        public double NextValue(double current, double target, double velocity) {
            return X.GetNextValue(current, target, velocity);
        }

    }
}
