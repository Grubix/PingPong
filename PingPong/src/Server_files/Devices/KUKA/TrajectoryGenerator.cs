using System;

namespace PingPong.KUKA {
    public class TrajectoryGenerator {

        private class Parameter {
            private double a0;
            private double a1;
            private double a2;
            private double a3;
            private double velocity;
            private double nextValue;

            public Parameter() {
                a0 = 0.0;
                a1 = 0.0;
                a2 = 0.0;
                a3 = 0.0;
                velocity = 0.0;
                nextValue = 0.0;
            }

            public void UpdateCoefficients(double currentPosition, double targetPosition, double targetVelocity, double time) {
                a0 = currentPosition;
                a1 = velocity;
                a2 = (3 * (targetPosition - currentPosition) - 2 * velocity * time - targetVelocity * time) / Math.Pow(time, 2);
                a3 = (targetVelocity * time + velocity * time - 2 * (targetPosition - currentPosition)) / Math.Pow(time, 3);
            }

            public void ComputeNextValue(double period) {
                nextValue = a3 * Math.Pow(period, 3) + a2 * Math.Pow(period, 2) + a1 * period + a0;
            }

            public void UpdateVelocity(double period) {
                velocity = 3 * a3 * Math.Pow(period, 2) + 2 * a2 * period + a1;
            }

            public double GetNextValue() {
                return nextValue;
            }
        }

        private readonly Parameter X = new Parameter();
        private readonly Parameter Y = new Parameter();
        private readonly Parameter Z = new Parameter();
        private readonly Parameter A = new Parameter();
        private readonly Parameter B = new Parameter();
        private readonly Parameter C = new Parameter();

        private readonly double period = 0.004;
        private double time2Dest = 0.0;
        private double totalTime2Dest = 0.0;
        private E6POS targetPosition;

        public TrajectoryGenerator(E6POS currentPosition) {
            targetPosition = currentPosition;
        }

        public E6POS GetNextPosition(E6POS currentPosition, E6POS targetPosition, double time) {
            if (currentPosition == targetPosition) {
                //TODO: wedlug mnie w tym miejscu powinien byc reset predkosci w kazdym wielomianie
                return targetPosition;
            }
            if (totalTime2Dest != time || this.targetPosition != targetPosition) {
                totalTime2Dest = time;
                time2Dest = time;
                this.targetPosition = targetPosition;
            }
            if (time2Dest >= 0.004) {
                UpdateCoefficients(currentPosition, targetPosition);
                ComputeNextPoint();
                time2Dest -= period;
                UpdateVelocity();

                return new E6POS(
                    X.GetNextValue(),
                    Y.GetNextValue(),
                    Z.GetNextValue(),
                    A.GetNextValue(),
                    B.GetNextValue(),
                    C.GetNextValue()
                );
            } else {
                //TODO: wedlug mnie w tym miejscu powinien byc reset predkosci w kazdym wielomianie
                totalTime2Dest = 0.0;
                return targetPosition;
            }
        }

        private void UpdateCoefficients(E6POS currentPosition, E6POS targetPosition) {
            // guessing targetVelocity == 0.0
            X.UpdateCoefficients(currentPosition.X, targetPosition.X, 0.0, time2Dest);
            Y.UpdateCoefficients(currentPosition.Y, targetPosition.Y, 0.0, time2Dest);
            Z.UpdateCoefficients(currentPosition.Z, targetPosition.Z, 0.0, time2Dest);
            A.UpdateCoefficients(currentPosition.A, targetPosition.A, 0.0, time2Dest);
            B.UpdateCoefficients(currentPosition.B, targetPosition.B, 0.0, time2Dest);
            C.UpdateCoefficients(currentPosition.C, targetPosition.C, 0.0, time2Dest);
        }

        private void ComputeNextPoint() {
            X.ComputeNextValue(period);
            Y.ComputeNextValue(period);
            Z.ComputeNextValue(period);
            A.ComputeNextValue(period);
            B.ComputeNextValue(period);
            C.ComputeNextValue(period);
        }

        private void UpdateVelocity() {
            X.UpdateVelocity(period);
            Y.UpdateVelocity(period);
            Z.UpdateVelocity(period);
            A.UpdateVelocity(period);
            B.UpdateVelocity(period);
            C.UpdateVelocity(period);
        }

    }
}
