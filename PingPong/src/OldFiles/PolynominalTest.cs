using System;

namespace PingPong {
    class Polynominal {

        private const double deltaTime = 0.004;

        private double a0, a1, a2, a3, a4, a5;

        private double prevTargetValue;

        private double prevTargetDuration;

        private double startVelocity;

        private double startAcceleration;

        private double elapsedTime;

        public Polynominal(double currentValue) {
            prevTargetValue = currentValue;
        }

        private void UpdateCoefficients(double startValue, double targetValue, double targetDuration) {
            double d1 = targetDuration;
            double d2 = d1 * d1;
            double d3 = d1 * d2;
            double d4 = d2 * d2;
            double d5 = d1 * d4;

            a0 = startValue;
            a1 = startVelocity;
            a2 = startAcceleration / 2;
            a3 = 1 / (2 * d3) * (-3 * startAcceleration * d2 - d1 * 12 * startVelocity + 20 * (targetValue - startValue));
            a4 = 1 / (2 * d4) * (3 * startAcceleration * d2 + d1 * 16 * startVelocity + 30 * (startValue - targetValue));
            a5 = 1 / (2 * d5) * (-1 * startAcceleration * d2 - d1 * 6 * startVelocity + 12 * (targetValue - startValue));
        }

        public double GetNextValue(double startValue, double targetValue, double targetDuration) {
            if (targetDuration <= 0) {
                throw new ArgumentException("Duration value must be grater than zero");
            }

            if (Math.Abs(targetValue - startValue) <= 0.001) {
                return startValue;
            }

            if (targetDuration != prevTargetDuration) {
                elapsedTime = 0;
                UpdateCoefficients(startValue, targetValue, targetDuration);
            } else {
                elapsedTime += deltaTime;
            }

            prevTargetValue = targetValue;
            prevTargetDuration = targetDuration;

            double t1 = elapsedTime;
            double t2 = t1 * t1;
            double t3 = t1 * t2;
            double t4 = t2 * t2;
            double t5 = t1 * t4;

            startVelocity = a1 + 2 * a2 * t1 + 3 * a3 * t2 + 4 * a4 * t3 + 5 * a5 * t4;
            startAcceleration = 2 * a2 + 6 * a3 * t1 + 12 * a4 * t2 + 20 * a5 * t3;

            return a0 + a1 * t1 + a2 * t2 + a3 * t3 + a4 * t4 + a5 * t5;
        }

    }
}
