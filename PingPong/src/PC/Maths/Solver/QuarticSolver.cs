using MathNet.Numerics;
using System.Collections.Generic;
using System.Numerics;

namespace PingPong.Maths.Solver {
    static class QuatricSolver {

        public static Complex[] Solve(double a, double b, double c, double d, double e) {
            if (a == 0.0) {
                return CubicSolver.Solve(b, c, d, e);
            }

            double B = b / a;
            double C = c / a;
            double D = d / a;
            double E = e / a;

            double B_2 = B * B;
            double B_3 = B * B_2;
            double B_4 = B * B_3;

            double f = C - (3.0 * B_2 / 8.0);
            double g = D + B_3 / 8.0 - B * C / 2.0;
            double h = E - 3 * B_4 / 256.0 + B_2 * C / 16.0 - B * D / 4.0;

            double ya = 1.0;
            double yb = f / 2.0;
            double yc = (f * f - 4.0 * h) / 16.0;
            double yd = -g * g / 64.0;

            Complex[] yRoots = CubicSolver.Solve(ya, yb, yc, yd);

            return FindRoots(yRoots, a, b, g);
        }

        public static double[] SolveReal(double a, double b, double c, double d, double e) {
            Complex[] roots = Solve(a, b, c, d, e);
            var realRoots = new List<double>();

            foreach (var root in roots) {
                if (root.Imaginary == 0.0) {
                    realRoots.Add(root.Real);
                }
            }

            realRoots.Sort();
            return realRoots.ToArray();
        }

        private static Complex[] FindRoots(Complex[] yRoots, double a, double b, double g) {
            Complex P, Q, R, S;

            if (yRoots[0].IsZero()) {
                P = yRoots[1];
                Q = yRoots[2];
            } else if (yRoots[1].IsZero()) {
                P = yRoots[0];
                Q = yRoots[2];
            } else if (yRoots[2].IsZero()) {
                P = yRoots[0];
                Q = yRoots[1];
            } else {
                if (yRoots[0].Imaginary != 0.0) {
                    P = yRoots[0];
                    Q = yRoots[1].Imaginary != 0.0 ? yRoots[1] : yRoots[2];
                } else if (yRoots[1].Imaginary != 0.0) {
                    P = yRoots[1];
                    Q = yRoots[0].Imaginary != 0.0 ? yRoots[0] : yRoots[2];
                } else if (yRoots[2].Imaginary != 0.0) {
                    P = yRoots[2];
                    Q = yRoots[0].Imaginary != 0.0 ? yRoots[0] : yRoots[1];
                } else {
                    P = yRoots[0];
                    Q = yRoots[1];
                }
            }

            P = P.SquareRoot();
            Q = Q.SquareRoot();
            R = -g / (8.0 * P * Q);
            S = new Complex(b / (4.0 * a), 0.0);

            return new Complex[] {
                P + Q + R - S,
                P - Q - R - S,
                Q - P - R - R,
                R - Q - P - S
            };
        }

    }
}
