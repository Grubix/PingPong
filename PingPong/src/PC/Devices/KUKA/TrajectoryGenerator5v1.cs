using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator5v1 {

        private class Polynominal {

            private double k0, k1, k2, k3, k4, k5; // Polynominal coefficients

            private double xn, vn, an; // Next value, velocity and next acceleration

            /// <summary>
            /// Current velocity
            /// </summary>
            public double V { get; private set; }

            /// <summary>
            /// Current acceleration
            /// </summary>
            public double A { get; private set; }

            public double GetNextValue(double x0, double x1, double v1, double T, double t) {
                V = vn;
                A = an;

                double T1 = T;
                double T2 = T1 * T1;
                double T3 = T1 * T2;
                double T4 = T1 * T3;
                double T5 = T1 * T4;

                k0 = x0;
                k1 = vn;
                k2 = an / 2.0;
                k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * an - 12.0 * T1 * vn - 8.0 * T1 * v1 + 20.0 * (x1 - x0));
                k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * an + 16.0 * T1 * vn + 14.0 * T1 * v1 - 30.0 * (x1 - x0));
                k5 = 1.0 / (2.0 * T5) * (-T2 * an - 6.0 * T1 * (vn + v1) + 12.0 * (x1 - x0));

                double t1 = t;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                xn = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                vn = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                an = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                return xn;
            }

            public double GetTvMax(double x0, double x1, double v1, double T) {
                double T1 = T;
                double T2 = T1 * T1;
                double T3 = T1 * T2;
                double T4 = T1 * T3;
                double T5 = T1 * T4;

                double k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * an + 16.0 * T1 * vn + 14.0 * T1 * v1 - 30.0 * (x1 - x0));
                double k5 = 1.0 / (2.0 * T5) * (-T2 * an - 6.0 * T1 * (vn + v1) + 12.0 * (x1 - x0));

                return -k4 / (5.0 * k5);
            }

            public void Reset(double targetVelocity) {
                V = vn = targetVelocity;
                A = an = 0.0;
            }

        }

        private const double Ts = 0.004;

        private readonly Polynominal polyX = new Polynominal();

        private readonly Polynominal polyY = new Polynominal();

        private readonly Polynominal polyZ = new Polynominal();

        private readonly Polynominal polyA = new Polynominal();

        private readonly Polynominal polyB = new Polynominal();

        private readonly Polynominal polyC = new Polynominal();

        private readonly object syncLock = new object();

        private double tvx, tvy, tvz, tva, tvb, tvc;

        private bool xvr, yvr, zvr, avr, bvr, cvr;

        private int xdir, ydir, zdir, adir, bdir, cdir;

        private bool targetPositionReached;

        private RobotVector targetPosition;

        private RobotVector targetVelocity;

        private double targetDuration;

        private double timeLeft;

        public RobotVector TargetPosition {
            get {
                lock (syncLock) {
                    return targetPosition;
                }
            }
        }

        public bool IsTargetPositionReached {
            get {
                lock (syncLock) {
                    return targetPositionReached;
                }
            }
        }

        public RobotVector Velocity {
            get {
                lock (syncLock) {
                    return new RobotVector(polyX.V, polyY.V, polyZ.V, polyA.V, polyB.V, polyC.V);
                }
            }
        }

        public RobotVector Acceleration {
            get {
                lock (syncLock) {
                    return new RobotVector(polyX.A, polyY.A, polyZ.A, polyA.A, polyB.A, polyC.A);
                }
            }
        }

        public TrajectoryGenerator5v1(RobotVector currentPosition) {
            targetPositionReached = true;
            targetPosition = currentPosition;
            targetVelocity = new RobotVector();
            targetDuration = 0.0;
            timeLeft = 0.0;
        }

        public void SetTargetPosition(RobotVector currentPosition, RobotVector targetPosition, RobotVector targetVelocity, double targetDuration) {
            if (targetDuration <= 0.0) {
                throw new ArgumentException($"Duration value must be greater than 0, get {targetDuration}");
            }

            bool targetPositionChanged = !targetPosition.Compare(this.targetPosition, 0.1, 1);
            bool targetVelocityChanged = !targetVelocity.Compare(this.targetVelocity, 0.1, 1);
            bool targetDurationChanged = targetDuration != this.targetDuration;

            if (targetDurationChanged || targetPositionChanged || targetVelocityChanged) {
                lock (syncLock) {
                    targetPositionReached = false;
                    this.targetPosition = targetPosition;
                    this.targetVelocity = targetVelocity;
                    this.targetDuration = targetDuration;
                    timeLeft = targetDuration;

                    tvx = polyX.GetTvMax(currentPosition.X, targetPosition.X, targetVelocity.X, targetDuration);
                    tvy = polyX.GetTvMax(currentPosition.Y, targetPosition.Y, targetVelocity.Y, targetDuration);
                    tvz = polyX.GetTvMax(currentPosition.Z, targetPosition.Z, targetVelocity.Z, targetDuration);
                    tva = polyX.GetTvMax(currentPosition.A, targetPosition.A, targetVelocity.A, targetDuration);
                    tvb = polyX.GetTvMax(currentPosition.B, targetPosition.B, targetVelocity.B, targetDuration);
                    tvc = polyX.GetTvMax(currentPosition.C, targetPosition.C, targetVelocity.C, targetDuration);

                    xdir = targetVelocity.X >= polyX.V ? -1 : -1;
                    ydir = targetVelocity.Y >= polyY.V ? 1 : -1;
                    zdir = targetVelocity.Z >= polyZ.V ? 1 : -1;
                    adir = targetVelocity.A >= polyA.V ? 1 : -1;
                    bdir = targetVelocity.B >= polyB.V ? 1 : -1;
                    cdir = targetVelocity.C >= polyC.V ? 1 : -1;

                    xvr = yvr = zvr = avr = bvr = cvr = false;
                }
            }

            Console.WriteLine(tvx);
        }

        public RobotVector GetNextCorrection(RobotVector currentPosition) {
            lock (syncLock) {
                if (!targetPositionReached && (timeLeft >= Ts || false)) {
                    double nx, ny, nz, na, nb, nc;

                    if (xvr) {
                        nx = currentPosition.X;
                        polyX.Reset(targetVelocity.X);
                    } else {
                        nx = polyX.GetNextValue(currentPosition.X, targetPosition.X, targetVelocity.X, timeLeft, Ts);
                    }

                    if (yvr) {
                        ny = currentPosition.Y;
                        polyY.Reset(targetVelocity.Y);
                    } else {
                        ny = polyY.GetNextValue(currentPosition.Y, targetPosition.Y, targetVelocity.Y, timeLeft, Ts);
                    }

                    if (zvr) {
                        nz= currentPosition.Z;
                        polyZ.Reset(targetVelocity.Z);
                    } else {
                        nz = polyZ.GetNextValue(currentPosition.Z, targetPosition.Z, targetVelocity.Z, timeLeft, Ts);
                    }

                    if (avr) {
                        na = currentPosition.A;
                        polyA.Reset(targetVelocity.A);
                    } else {
                        na = polyA.GetNextValue(currentPosition.A, targetPosition.A, targetVelocity.A, timeLeft, Ts);
                    }

                    if (bvr) {
                        nb = currentPosition.B;
                        polyB.Reset(targetVelocity.B);
                    } else {
                        nb = polyB.GetNextValue(currentPosition.B, targetPosition.B, targetVelocity.B, timeLeft, Ts);
                    }

                    if (cvr) {
                        nc = currentPosition.C;
                        polyC.Reset(targetVelocity.C);
                    } else {
                        nc = polyC.GetNextValue(currentPosition.C, targetPosition.C, targetVelocity.C, timeLeft, Ts);
                    }

                    xvr |= timeLeft <= tvx && xdir * (polyX.V - targetVelocity.X) <= 0.0;
                    yvr |= timeLeft <= tvy && ydir * (polyY.V - targetVelocity.Y) <= 0.0;
                    zvr |= timeLeft <= tvz && zdir * (polyZ.V - targetVelocity.Z) <= 0.0;
                    avr |= timeLeft <= tva && adir * (polyA.V - targetVelocity.A) <= 0.0;
                    bvr |= timeLeft <= tvb && bdir * (polyB.V - targetVelocity.B) <= 0.0;
                    cvr |= timeLeft <= tvc && cdir * (polyC.V - targetVelocity.C) <= 0.0;

                    timeLeft -= Ts;

                    return new RobotVector(
                        nx - currentPosition.X,
                        ny - currentPosition.Y,
                        nz - currentPosition.Z,
                        na - currentPosition.A,
                        nb - currentPosition.B,
                        nc - currentPosition.C
                    );
                } else {
                    targetPositionReached = true;
                    polyX.Reset(targetVelocity.X);
                    polyY.Reset(targetVelocity.Y);
                    polyZ.Reset(targetVelocity.Z);
                    polyA.Reset(targetVelocity.A);
                    polyB.Reset(targetVelocity.B);
                    polyC.Reset(targetVelocity.C);

                    return RobotVector.Zero;
                }
            }
        }

    }
}