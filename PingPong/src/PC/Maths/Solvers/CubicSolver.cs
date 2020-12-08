using System;
using System.Numerics;

namespace PingPong.Maths.Solver {
    static class CubicSolver {

        public static Complex[] Solve(double a, double b, double c, double d) {
            if (a == 0.0) {
                return QuadraticSolver.Solve(b, c, d);
            }

            b /= a;
            c /= a;
            d /= a;

            Complex x1, x2, x3;

            double Q = (3.0 * c - b * b) / 9.0;
            double R = (b * (9.0 * c - 2.0 * b * b) - 27.0 * d) / 54.0;
            double D = Q * Q * Q + R * R;
            double term1 = b / 3.0;

            if (D > 0) {
                double DSqrt = Math.Sqrt(D);

                double s = R + DSqrt;
                s = (s < 0) ? -Math.Pow(-s, 1.0 / 3.0) : Math.Pow(s, 1.0 / 3.0);

                double t = R - DSqrt;
                t = (t < 0) ? -Math.Pow(-t, 1.0 / 3.0) : Math.Pow(t, 1.0 / 3.0);

                x1 = new Complex(-term1 + s + t, 0.0);
                x2 = new Complex(-(term1 + (s + t) / 2.0), Math.Sqrt(3.0) * (s - t) / 2.0);
                x3 = new Complex(x2.Real, -x2.Imaginary);
            } else if (D == 0) {
                double r13 = (R < 0) ? -Math.Pow(-R, 1.0 / 3.0) : Math.Pow(R, 1.0 / 3.0);

                x1 = new Complex(2.0 * r13 - term1, 0.0);
                x2 = new Complex(-(r13 + term1), 0.0);
                x3 = new Complex(x2.Real, 0.0);
            } else {
                Q = -Q;
                double r13 = 2.0 * Math.Sqrt(Q);
                double dum1 = Math.Acos(R / Math.Sqrt(Q * Q * Q));

                x1 = new Complex(-term1 + r13 * Math.Cos(dum1 / 3.0), 0.0);
                x2 = new Complex(-term1 + r13 * Math.Cos((dum1 + 2.0 * Math.PI) / 3.0), 0.0);
                x3 = new Complex(-term1 + r13 * Math.Cos((dum1 + 4.0 * Math.PI) / 3.0), 0.0);
            }

            return new Complex[] {
                x1, x2, x3
            };
        }

    }
}
