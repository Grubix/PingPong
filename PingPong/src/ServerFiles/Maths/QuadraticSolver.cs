using System;
using System.Collections.Generic;
using System.Numerics;

namespace PingPong {
    static class QuadraticSolver {

        public static Complex[] Solve(double a, double b, double c) {
            if (a == 0.0) {
                return new Complex[] { new Complex(-c / b, 0.0) };
            }

            Complex x1, x2;
            double delta = b * b - 4 * a * c;

            if (delta >= 0) {
                double deltaSqrt = Math.Sqrt(delta);

                x1 = new Complex((-b + deltaSqrt) / (2.0 * a), 0.0);
                x2 = new Complex((-b - deltaSqrt) / (2.0 * a), 0.0);
            } else {
                double absDeltaSqrt = Math.Sqrt(Math.Abs(delta));
                double realPart = -b / (2.0 * a);

                x1 = new Complex(realPart, absDeltaSqrt);
                x2 = new Complex(realPart, -absDeltaSqrt);
            }

            return new Complex[] { x1, x2 };
        }

        public static double[] SolveReal(double a, double b, double c) {
            Complex[] roots = Solve(a, b, c);
            var realRoots = new List<double>();

            foreach (var root in roots) {
                if (root.Imaginary == 0.0) {
                    realRoots.Add(root.Real);
                }
            }

            realRoots.Sort();
            return realRoots.ToArray();
        }

    }
}
