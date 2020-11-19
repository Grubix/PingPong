using System;

namespace PingPong.KUKA {
    public class E6AXIS {

        public double A1 { get; }

        public double A2 { get; }

        public double A3 { get; }

        public double A4 { get; }

        public double A5 { get; }

        public double A6 { get; }

        public E6AXIS(double A1, double A2, double A3, double A4, double A5, double A6) {
            this.A1 = A1;
            this.A2 = A2;
            this.A3 = A3;
            this.A4 = A4;
            this.A5 = A5;
            this.A6 = A6;
        }

        public override string ToString() {
            return
                $"X={Math.Round(A1 * 1000) / 1000}, " +
                $"Y={Math.Round(A2 * 1000) / 1000}, " +
                $"Z={Math.Round(A3 * 1000) / 1000}, " +
                $"A={Math.Round(A4 * 1000) / 1000}, " +
                $"B={Math.Round(A5 * 1000) / 1000}, " +
                $"C={Math.Round(A6 * 1000) / 1000}";
        }

    }
}
