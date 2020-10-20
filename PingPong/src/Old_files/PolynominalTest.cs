using System;

namespace PingPong {
    class Polynominal {

        private const double deltaTime = 0.004;

        private double a0, a1, a2, a3, a4, a5;

        private double previousTargetValue;

        private double previousTargetDuration;

        private double startVelocity;

        private double startAcceleration;

        private double elapsedTime;

        public Polynominal(double currentValue) {
            previousTargetValue = currentValue;
        }

        private void UpdateCoefficients(double startValue, double targetValue, double targetDuration) {
            double T1 = targetDuration;
            double T2 = Math.Pow(targetDuration, 2);
            double T3 = Math.Pow(targetDuration, 3);
            double T4 = Math.Pow(targetDuration, 4);
            double T5 = Math.Pow(targetDuration, 5);

            a0 = startValue;
            a1 = startVelocity;
            a2 = startAcceleration / 2;
            a3 = 1 / (2 * T3) * (-3 * startAcceleration * T2 - T1 * 12 * startVelocity + 20 * (targetValue - startValue));
            a4 = 1 / (2 * T4) * (+3 * startAcceleration * T2 + T1 * 16 * startVelocity + 30 * (startValue - targetValue));
            a5 = 1 / (2 * T5) * (-1 * startAcceleration * T2 - T1 * 6 * startVelocity + 12 * (targetValue - startValue));
        }

        public double GoTo(double startValue, double targetValue, double targetDuration) {
            if (targetDuration <= 0) {
                throw new Exception();
            }

            if (Math.Abs(targetValue - startValue) <= 0.001) {
                return startValue; // return 0;
            }

            if (Math.Abs(targetValue - previousTargetValue) >= 0.001 || Math.Abs(targetDuration - previousTargetDuration) >= 0.001) {
                //start new movement
                elapsedTime = 0;
                UpdateCoefficients(startValue, targetValue, targetDuration);
            } else {
                elapsedTime += deltaTime;
            }

            previousTargetValue = targetValue;
            previousTargetDuration = targetDuration;

            double t1 = elapsedTime;
            double t2 = Math.Pow(elapsedTime, 2);
            double t3 = Math.Pow(elapsedTime, 3);
            double t4 = Math.Pow(elapsedTime, 4);
            double t5 = Math.Pow(elapsedTime, 5);

            startVelocity = a1 + 2 * a2 * t1 + 3 * a3 * t2 + 4 * a4 * t3 + 5 * a5 * t4;
            startAcceleration = 2 * a2 + 6 * a3 * t1 + 12 * a4 * t2 + 20 * a5 * t3;

            return a0 + a1 * t1 + a2 * t2 + a3 * t3 + a4 * t4 + a5 * t5;
        }

    }
}
