using System;

namespace PingPong.Maths {
    class CubicSolver {

        public (double re, double im)[] Solve(double a, double b, double c, double d) {
            b /= a;
            c /= a;
            d /= a;

            (double re, double im) x1 = (0.0, 0.0);
            (double re, double im) x2 = (0.0, 0.0);
            (double re, double im) x3 = (0.0, 0.0);

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

                x1.re = -term1 + s + t;
                x2.re = x3.re = -(term1 + (s + t) / 2.0);

                x1.im = 0.0;
                x2.im = Math.Sqrt(3.0) * (s - t) / 2.0;
                x3.im = -x2.im;
            } else if (D == 0) {
                double r13 = (R < 0) ? -Math.Pow(-R, 1.0 / 3.0) : Math.Pow(R, 1.0 / 3.0);

                x1.re = 2.0 * r13 - term1;
                x2.re = x3.re = -(r13 + term1);

                x1.im = 0.0;
                x2.im = 0.0;
                x3.im = 0.0;
            } else {
                Q = -Q;
                double r13 = 2.0 * Math.Sqrt(Q);
                double dum1 = Math.Acos(R / Math.Sqrt(Q *Q * Q));

                x1.im = -term1 + r13 * Math.Cos(dum1 / 3.0);
                x2.im = -term1 + r13 * Math.Cos((dum1 + 2.0 * Math.PI) / 3.0);
                x3.im = -term1 + r13 * Math.Cos((dum1 + 4.0 * Math.PI) / 3.0);

                x1.im = 0.0;
                x2.im = 0.0;
                x3.im = 0.0;
            }

            return new (double re, double im)[] {
                x1, x2, x3
            };
        }

    }
}
