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

            private double correction;

            public Parameter() {
                a0 = 0.0;
                a1 = 0.0;
                a2 = 0.0;
                a3 = 0.0;
                correction = 0.0;
            }

            public double GetUpdatedVelocity(double period) {
                velocity =  3 * a3 * Math.Pow(period, 2) + 2 * a2 * period + a1;
                return velocity;
            }

            public void UpdateCoefficients(double currentPosition, double targetPosition, double currentVelocity, double targetVelocity, double period) {
                a0 = currentPosition;
                a1 = currentVelocity;
                a2 = (3 * (targetPosition - currentPosition) - 2 * currentVelocity * period - targetVelocity * period) / Math.Pow(period, 2);
                a3 = (targetVelocity * period + currentVelocity * period - 2 * (targetPosition - currentPosition)) / Math.Pow(period, 3);
            }

            public void ComputeCorrection(double period) {
                correction = a3 * Math.Pow(period, 3) + a2 * Math.Pow(period, 2) + a1 * period;
            }

            public double GetCorrection() {
                return correction;
            }
        }

        private readonly Parameter X = new Parameter();
        private readonly Parameter Y = new Parameter();
        private readonly Parameter Z = new Parameter();
        private readonly Parameter A = new Parameter();
        private readonly Parameter B = new Parameter();
        private readonly Parameter C = new Parameter();
        
        private double period = 0.004;
        public double timeToDest = -1.0;
        public Vector<double> currentVelocity;
        Vector<double> targetVelocity;

        private E6POS targetPosition = new E6POS();

        public TrajectoryGenerator() {
            currentVelocity = Vector<double>.Build.Dense(6);
            currentVelocity.Clear();
            targetVelocity = Vector<double>.Build.Dense(6);
            targetVelocity.Clear();
        }

        public E6POS GoToPoint(E6POS currentPosition, E6POS targetPosition, double time) {
            if(this.targetPosition != targetPosition) {
                this.targetPosition = targetPosition;
                timeToDest = -1.0;
            }

            if (timeToDest == -1.0) {
                timeToDest = time;
            }
            if (timeToDest >= 0.004) {
                UpdateCoefficients(currentPosition, this.targetPosition, currentVelocity, targetVelocity, timeToDest);
                ComputeCorrection(period);
                timeToDest -= period;
                UpdateVelocity(period);
                return GetCorrection();
            } else {
                timeToDest = -1.0;
                return new E6POS();
            }
        }

        public void UpdateVelocity(double period) {
            currentVelocity[0] = X.GetUpdatedVelocity(period);
            currentVelocity[1] = Y.GetUpdatedVelocity(period);
            currentVelocity[2] = Z.GetUpdatedVelocity(period);
            currentVelocity[3] = A.GetUpdatedVelocity(period);
            currentVelocity[4] = B.GetUpdatedVelocity(period);
            currentVelocity[5] = C.GetUpdatedVelocity(period);
            //Console.WriteLine("velocity: " + currentVelocity[5]);
            
        }

        public void UpdateCoefficients(E6POS currentPosition, E6POS targetPosition, Vector<double> currentVelocity, Vector<double> targetVelocity, double period) {
            X.UpdateCoefficients(currentPosition.X, targetPosition.X, currentVelocity[0], targetVelocity[0], period);
            Y.UpdateCoefficients(currentPosition.Y, targetPosition.Y, currentVelocity[1], targetVelocity[1], period);
            Z.UpdateCoefficients(currentPosition.Z, targetPosition.Z, currentVelocity[2], targetVelocity[2], period);
            A.UpdateCoefficients(currentPosition.A, targetPosition.A, currentVelocity[3], targetVelocity[3], period);
            B.UpdateCoefficients(currentPosition.B, targetPosition.B, currentVelocity[4], targetVelocity[4], period);
            C.UpdateCoefficients(currentPosition.C, targetPosition.C, currentVelocity[5], targetVelocity[5], period);
        }

        public void ComputeCorrection(double period) {
            X.ComputeCorrection(period);
            Y.ComputeCorrection(period);
            Z.ComputeCorrection(period);
            A.ComputeCorrection(period);
            B.ComputeCorrection(period);
            C.ComputeCorrection(period);
        }

        public E6POS GetCorrection() {
            return new E6POS(
                X.GetCorrection(),
                Y.GetCorrection(),
                Z.GetCorrection(),
                A.GetCorrection(),
                B.GetCorrection(),
                C.GetCorrection()
            );
        }
    }
}
