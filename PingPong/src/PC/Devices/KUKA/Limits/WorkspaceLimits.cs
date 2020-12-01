using System;

namespace PingPong.KUKA {
    class WorkspaceLimits {

        private readonly double rMin, rMax;

        private readonly double zMin, zMax;

        private readonly double gammaMin, gammaMax;

        public double X0 { get; }

        public double Y0 { get; }

        public double Z0 { get; }

        public double Alpha { get; }

        public double Beta { get; }

        public double R1 { get; }

        public double R2 { get; }

        public double H { get; }

        public WorkspaceLimits(double x0, double y0, double z0, double alpha, double beta, double r1, double r2, double h) {
            r1 = Math.Abs(r1);
            r2 = Math.Abs(r2);
            h = Math.Abs(h);
            
            X0 = x0;
            Y0 = y0;
            Z0 = z0;
            Alpha = alpha;
            Beta = beta;
            R1 = r1;
            R2 = r2;
            H = h;

            rMin = r1;
            rMax = r2;

            zMin = z0 - h / 2.0;
            zMax = z0 + h / 2.0;

            gammaMin = alpha - beta / 2.0;
            gammaMax = alpha + beta / 2.0;
        }

        public bool CheckPosition(E6POS position) {
            double px = position.X - X0;
            double py = position.Y - Y0;

            double L = Math.Sqrt(px * px + py * py);
            double gamma = Math.Atan2(py, px);

            bool c1 = L >= rMin && L <= rMax;
            bool c2 = position.Z >= zMin && position.Z <= zMax;
            bool c3 = gamma >= gammaMin && gamma <= gammaMax;

            return c1 && c2 && c3;
        }

    }
}
