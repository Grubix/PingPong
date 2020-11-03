using System;

namespace PingPong.KUKA {
    public class RobotLimits {

        public (double min, double max) LimitX { get; set; } = (-1000, 1000);
        public (double min, double max) LimitY { get; set; } = (-1000, 1000);
        public (double min, double max) LimitZ { get; set; } = (-1000, 1000);

        public (double min, double max) LimitA1 { get; set; } = (-180.0, 180.0);
        public (double min, double max) LimitA2 { get; set; } = (-180.0, 180.0);
        public (double min, double max) LimitA3 { get; set; } = (-180.0, 180.0);
        public (double min, double max) LimitA4 { get; set; } = (-180.0, 180.0);
        public (double min, double max) LimitA5 { get; set; } = (-180.0, 180.0);
        public (double min, double max) LimitA6 { get; set; } = (-180.0, 180.0);

        public (double maxXYZ, double maxABC) LimitCorrection { get; set; } = (0.5, 0.05);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns></returns>
        public bool CheckPosition(E6POS position) {
            return
                position.X >= LimitX.min && position.X <= LimitX.max &&
                position.Y >= LimitY.min && position.Y <= LimitY.max &&
                position.Z >= LimitZ.min && position.Z <= LimitZ.max;
        }

        public bool CheckAxisPosition(E6AXIS axisPosition) {
            return
                axisPosition.A1 >= LimitA1.min && axisPosition.A1 <= LimitA1.max &&
                axisPosition.A2 >= LimitA2.min && axisPosition.A2 <= LimitA2.max &&
                axisPosition.A3 >= LimitA3.min && axisPosition.A3 <= LimitA3.max &&
                axisPosition.A4 >= LimitA4.min && axisPosition.A4 <= LimitA4.max &&
                axisPosition.A5 >= LimitA5.min && axisPosition.A5 <= LimitA5.max &&
                axisPosition.A6 >= LimitA6.min && axisPosition.A6 <= LimitA6.max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correction">correction to check</param>
        /// <returns></returns>
        public bool CheckCorrection(E6POS correction) {
            return
                Math.Abs(correction.X) <= LimitCorrection.maxXYZ &&
                Math.Abs(correction.Y) <= LimitCorrection.maxXYZ &&
                Math.Abs(correction.Z) <= LimitCorrection.maxXYZ &&
                Math.Abs(correction.A) <= LimitCorrection.maxABC &&
                Math.Abs(correction.B) <= LimitCorrection.maxABC &&
                Math.Abs(correction.C) <= LimitCorrection.maxABC;
        }

    }
}
