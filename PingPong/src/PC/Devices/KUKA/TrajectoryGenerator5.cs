using MathNet.Numerics.LinearAlgebra;
using System;

namespace PingPong.KUKA {
    class TrajectoryGenerator5 {

        private class Polynominal {

            private double k0, k1, k2, k3, k4, k5; // Polynominal coefficients

            private double Vn, An; // Next value, velocity and next acceleration

            /// <summary>
            /// Current velocity
            /// </summary>
            public double V { get; private set; }

            /// <summary>
            /// Current acceleration
            /// </summary>
            public double A { get; private set; }

            public double GetNextValue(double x0, double x1, double v1, double T, double t) {
                V = Vn;
                A = An;

                double T1 = T;
                double T2 = T1 * T1;
                double T3 = T1 * T2;
                double T4 = T1 * T3;
                double T5 = T1 * T4;

                k0 = x0;
                k1 = Vn;
                k2 = An / 2.0;
                k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * An - 12.0 * T1 * Vn - 8.0 * T1 * v1 + 20.0 * (x1 - x0));
                k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * An + 16.0 * T1 * Vn + 14.0 * T1 * v1 - 30.0 * (x1 - x0));
                k5 = 1.0 / (2.0 * T5) * (-T2 * An - 6.0 * T1 * (Vn + v1) + 12.0 * (x1 - x0));

                double t1 = t;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                double nextValue = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                Vn = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                An = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                return nextValue;
            }

            public void Reset() {
                V = Vn = A = An = 0.0;
            }

        }

        private readonly Polynominal polyX = new Polynominal();

        private readonly Polynominal polyY = new Polynominal();

        private readonly Polynominal polyZ = new Polynominal();

        private readonly Polynominal polyA = new Polynominal();

        private readonly Polynominal polyB = new Polynominal();

        private readonly Polynominal polyC = new Polynominal();

        private readonly object syncLock = new object();

        private bool targetPositionReached;

        private RobotVector targetPosition;

        private RobotVector targetVelocity;

        private double targetDuration;

        private double timeLeft;

        private const double Ts = 0.004;

        public RobotVector TargetPosition {
           get {
                lock (syncLock) {
                    return targetPosition;
                }
            }
        }

        public bool TargetPositionReached { 
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

        public TrajectoryGenerator5(RobotVector currentPosition) {
            targetPositionReached = true;
            targetPosition = currentPosition;
            targetDuration = 0.0;
            timeLeft = 0.0;
        }

        public void SetTargetPosition(RobotVector targetPosition, RobotVector targetVelocity, double targetDuration) {
            if (targetDuration <= 0.0) {
                throw new ArgumentException($"Duration value must be greater than 0, get {targetDuration}");
            }

            bool targetPositionChanged = !targetPosition.Compare(this.targetPosition, 0.1, 1);
            bool targetVelocityChanged = !targetVelocity.Compare(this.targetVelocity, 0.1, 1);
            //TODO: sprawdzenie targetVelocity czy sie zmienilo
            bool targetDurationChanged = targetDuration != this.targetDuration;

            if (targetDurationChanged || targetPositionChanged || targetVelocityChanged) {
                lock (syncLock) {
                    targetPositionReached = false;
                    this.targetPosition = targetPosition;
                    this.targetVelocity = targetVelocity;
                    this.targetDuration = targetDuration;
                    timeLeft = targetDuration;
                }
            }
        }

        public RobotVector GetNextCorrection(RobotVector currentPosition) {
            lock (syncLock) {
                if (timeLeft >= Ts) {
                    double nx = polyX.GetNextValue(currentPosition.X, targetPosition.X, targetVelocity.X, timeLeft, Ts);
                    double ny = polyY.GetNextValue(currentPosition.Y, targetPosition.Y, targetVelocity.Y, timeLeft, Ts);
                    double nz = polyZ.GetNextValue(currentPosition.Z, targetPosition.Z, targetVelocity.Z, timeLeft, Ts);
                    double na = polyA.GetNextValue(currentPosition.A, targetPosition.A, targetVelocity.A, timeLeft, Ts);
                    double nb = polyB.GetNextValue(currentPosition.B, targetPosition.B, targetVelocity.B, timeLeft, Ts);
                    double nc = polyC.GetNextValue(currentPosition.C, targetPosition.C, targetVelocity.C, timeLeft, Ts);

                    RobotVector nextPosition = new RobotVector(nx, ny, nz, na, nb, nc);
                    timeLeft -= Ts;

                    return nextPosition - currentPosition;
                } else {
                    targetPositionReached = true;
                    polyX.Reset();
                    polyY.Reset();
                    polyZ.Reset();
                    polyA.Reset();
                    polyB.Reset();
                    polyC.Reset();

                    return new RobotVector();
                }
            }
        }

    }
}