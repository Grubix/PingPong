namespace PingPong.Maths {
    /// <summary>
    /// https://www.scilab.org/discrete-time-pid-controller-implementation
    /// </summary>
    class PIDRegulator {

        private double kp, ki, kd, ts, n;

        private double ku1, ku2, ke0, ke1, ke2;

        private double u0, u1, u2; // u0 = u[k]; u1 = u[k-1]; u2 = u[k-2] ## OUTPUT

        private double e1, e2; // e1 = e[k-1]; e2 = e[k-2] ## ERROR (setpoint - feedback)

        public double Setpoint { get; set; }

        public double Kp {
            get {
                return kp;
            }
            set {
                kp = value;
                CalculateCoefficients();
            }
        }

        public double Ki {
            get {
                return ki;
            }
            set {
                ki = value;
                CalculateCoefficients();
            }
        }

        public double Kd {
            get {
                return kd;
            }
            set {
                kd = value;
                CalculateCoefficients();
            }
        }

        public double Ts {
            get {
                return ts;
            }
            set {
                ts = value;
                CalculateCoefficients();
            }
        }

        public double N {
            get {
                return n;
            }
            set {
                n = value;
                CalculateCoefficients();
            }
        }

        public PIDRegulator(double kp, double ki, double kd, double N, double Ts, double setpoint) {
            Setpoint = setpoint;
            this.kp = kp;
            this.ki = ki;
            this.kd = kd;
            ts = Ts;
            n = N;

            CalculateCoefficients();
        }

        private void CalculateCoefficients() {
            double a0 = 1 + N * Ts;
            double a1 = -(2.0 + N * Ts);
            double a2 = 1.0;

            double b0 = Kp * (1.0 + N * Ts) + Ki * Ts * (1.0 + N * Ts) + Kd * N;
            double b1 = -(Kp * (2.0 + N * Ts) + Ki * Ts + 2.0 * Kd * N);
            double b2 = Kp + Kd * N;

            ku1 = -a1 / a0;
            ku2 = -a2 / a0;
            ke0 = b0 / a0;
            ke1 = b1 / a0;
            ke2 = b2 / a0;
        }

        public double Compute(double feedback) {
            double e0 = Setpoint - feedback;

            e2 = e1;
            e1 = e0;
            u2 = u1;
            u1 = u0;

            u0 = -(ku1 * u1) - (ku2 * u2) + (ke0 * e0) + (ke1 * e1) + (ke2 * e2);

            //TODO: limity wyjscia, anti windup ?

            return u0;
        }

    }
}
