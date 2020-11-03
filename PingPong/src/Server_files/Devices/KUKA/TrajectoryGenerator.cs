using MathNet.Numerics.LinearAlgebra;
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
                nextValue = a3 * Math.Pow(period, 3) + a2 * Math.Pow(period, 2) + a1 * period;
            }

            public void UpdateVelocity(double period) {
                velocity = 3 * a3 * Math.Pow(period, 2) + 2 * a2 * period + a1;
            }

            public double GetNextValue() {
                return nextValue;
            }
			
			public void ResetVelocity() {
				velocity = 0.0;
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

        public E6POS GetNextCorrection(E6POS currentPosition, E6POS targetPosition, double time) {
            if (currentPosition == targetPosition) {
				ResetVelocity();
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
                totalTime2Dest = 0.0;
				ResetVelocity();
                return targetPosition;
            }
        }

        private void UpdateCoefficients(E6POS currentPosition, E6POS targetPosition) {
            // guessing targetVelocity == 0.0
            X.UpdateCoefficients(currentPosition.X, targetPosition.X, 0.0, time2Dest);
            Y.UpdateCoefficients(currentPosition.Y, targetPosition.Y, 0.0, time2Dest);
            Z.UpdateCoefficients(currentPosition.Z, targetPosition.Z, 0.0, time2Dest);

            Vector<double> currentABC = currentPosition.ABC;
            Vector<double> targetABC = targetPosition.ABC;

            // handling passing through +-180
			if (targetABC[0] - currentABC[0] > 180.0 || targetABC[0] - currentABC[0] < -180.0) {
				currentABC[0] = (currentABC[0] + 360.0) % 360 - currentABC[0];
				targetABC[0] = (targetABC[0] + 360.0) % 360 - targetABC[0];
			}

            if (targetABC[1] - currentABC[1] > 180.0 || targetABC[1] - currentABC[1] < -180.0) {
				currentABC[1] = (currentABC[1] + 360.0) % 360 - currentABC[1];
				targetABC[1] = (targetABC[1] + 360.0) % 360 - targetABC[1];
			}

            if (targetABC[2] - currentABC[2] > 180.0 || targetABC[2] - currentABC[2] < -180.0) {
				currentABC[2] = (currentABC[2] + 360.0) % 360 - currentABC[2];
				targetABC[2] = (targetABC[2] + 360.0) % 360 - targetABC[2];
			}

            currentPosition += new E6POS(0.0, 0.0, 0.0, currentABC[0], currentABC[1], currentABC[2]);
            targetPosition += new E6POS(0.0, 0.0, 0.0, targetABC[0], targetABC[1], targetABC[2]);

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
		
		private void ResetVelocity() {
			X.ResetVelocity();
			Y.ResetVelocity();
			Z.ResetVelocity();
			A.ResetVelocity();
			B.ResetVelocity();
			C.ResetVelocity();
		}

    }
}
