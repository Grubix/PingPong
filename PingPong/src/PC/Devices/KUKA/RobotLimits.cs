using System;

namespace PingPong.KUKA {
    public class RobotLimits {

        public (double Min, double max) LimitX { get; }
        public (double Min, double max) LimitY { get; }
        public (double Min, double max) LimitZ { get; }

        public (double Min, double max) LimitA1 { get; }
        public (double Min, double max) LimitA2 { get; }
        public (double Min, double max) LimitA3 { get; }
        public (double Min, double max) LimitA4 { get; }
        public (double Min, double max) LimitA5 { get; }
        public (double Min, double max) LimitA6 { get; }

        public (double XYZ, double ABC) MaxCorrection { get; }
        public (double XYZ, double ABC) MaxVelocity { get; }

        public (double X, double Y, double Z) LowerWorkspaceLimit { get; }
        public (double X, double Y, double Z) UpperWorkspaceLimit { get; }

        public RobotLimits(
            (double Min, double max) limitX, (double Min, double max) limitY, (double Min, double max) limitZ,
            (double Min, double max) limitA1, (double Min, double max) limitA2, (double Min, double max) limitA3, 
            (double Min, double max) limitA4, (double Min, double max) limitA5, (double Min, double max) limitA6,
            (double XYZ, double ABC) maxCorrection) {

            LimitX = limitX;
            LimitY = limitY;
            LimitZ = limitZ;

            LimitA1 = limitA1;
            LimitA2 = limitA2;
            LimitA3 = limitA3;
            LimitA4 = limitA4;
            LimitA5 = limitA5;
            LimitA6 = limitA6;

            MaxCorrection = maxCorrection;
            MaxVelocity = (maxCorrection.XYZ / 0.004, maxCorrection.ABC / 0.004);

            LowerWorkspaceLimit = (LimitX.Min, LimitY.Min, LimitZ.Min);
            LowerWorkspaceLimit = (LimitX.max, LimitY.max, LimitZ.max);
        }

        public bool CheckPosition(E6POS position) {
            bool checkX = position.X >= LimitX.Min && position.X <= LimitX.max;
            bool checkY = position.Y >= LimitY.Min && position.Y <= LimitY.max;
            bool checkZ = position.Z >= LimitZ.Min && position.Z <= LimitZ.max;
            return checkX && checkY && checkZ;
        }

        public bool CheckAxisPosition(E6AXIS axisPosition) {
            bool checkA1 = axisPosition.A1 >= LimitA1.Min && axisPosition.A1 <= LimitA1.max;
            bool checkA2 = axisPosition.A2 >= LimitA2.Min && axisPosition.A2 <= LimitA2.max;
            bool checkA3 = axisPosition.A3 >= LimitA3.Min && axisPosition.A3 <= LimitA3.max;
            bool checkA4 = axisPosition.A4 >= LimitA4.Min && axisPosition.A4 <= LimitA4.max;
            bool checkA5 = axisPosition.A5 >= LimitA5.Min && axisPosition.A5 <= LimitA5.max;
            bool checkA6 = axisPosition.A6 >= LimitA6.Min && axisPosition.A6 <= LimitA6.max;
            return checkA1 && checkA2 && checkA3 && checkA4 && checkA5 && checkA6;
        }

        public bool CheckCorrection(E6POS correction) {
            bool checkX = Math.Abs(correction.X) <= MaxCorrection.XYZ;
            bool checkY = Math.Abs(correction.Y) <= MaxCorrection.XYZ;
            bool checkZ = Math.Abs(correction.Z) <= MaxCorrection.XYZ;
            bool checkA = Math.Abs(correction.A) <= MaxCorrection.ABC;
            bool checkB = Math.Abs(correction.B) <= MaxCorrection.ABC;
            bool checkC = Math.Abs(correction.C) <= MaxCorrection.ABC;
            return checkX && checkY && checkZ && checkA && checkB && checkC;
        }


        //public (bool result, string errorMessage) CheckPosition(E6POS position) {
        //    string errorMessage = "";

        //    bool checkX = position.X >= LimitX.Min && position.X <= LimitX.max;
        //    bool checkY = position.Y >= LimitY.Min && position.Y <= LimitY.max;
        //    bool checkZ = position.Z >= LimitZ.Min && position.Z <= LimitZ.max;
        //    bool result = checkX && checkY && checkZ;

        //    if (!result) {
        //        errorMessage = $"Workspace limit has been exceeded:{Environment.NewLine}";

        //        if (!checkX) {
        //            errorMessage += $"X: {position.X} [{LimitX.Min}, {LimitX.max}]{Environment.NewLine}";
        //        }

        //        if (!checkY) {
        //            errorMessage += $"Y: {position.Y} [{LimitY.Min}, {LimitY.max}]{Environment.NewLine}";
        //        }

        //        if (!checkZ) {
        //            errorMessage += $"Z: {position.Z} [{LimitZ.Min}, {LimitZ.max}]{Environment.NewLine}";
        //        }
        //    }

        //    return (result, errorMessage);
        //}

        //public (bool result, string errorMessage) CheckAxisPosition(E6AXIS axisPosition) {
        //    string errorMessage = "";

        //    bool checkA1 = axisPosition.A1 >= LimitA1.Min && axisPosition.A1 <= LimitA1.max;
        //    bool checkA2 = axisPosition.A2 >= LimitA2.Min && axisPosition.A2 <= LimitA2.max;
        //    bool checkA3 = axisPosition.A3 >= LimitA3.Min && axisPosition.A3 <= LimitA3.max;
        //    bool checkA4 = axisPosition.A4 >= LimitA4.Min && axisPosition.A4 <= LimitA4.max;
        //    bool checkA5 = axisPosition.A5 >= LimitA5.Min && axisPosition.A5 <= LimitA5.max;
        //    bool checkA6 = axisPosition.A6 >= LimitA6.Min && axisPosition.A6 <= LimitA6.max;
        //    bool result = checkA1 && checkA2 && checkA3 && checkA4 && checkA5 && checkA6;

        //    if (!result) {
        //        errorMessage = $"Axis position has been exceeded:{Environment.NewLine}";

        //        if (!checkA1) {
        //            errorMessage += $"X: {axisPosition.A1} [{LimitA1.Min}, {LimitA1.max}]{Environment.NewLine}";
        //        }

        //        if (!checkA2) {
        //            errorMessage += $"Y: {axisPosition.A2} [{LimitA2.Min}, {LimitA2.max}]{Environment.NewLine}";
        //        }

        //        if (!checkA3) {
        //            errorMessage += $"Z: {axisPosition.A3} [{LimitA3.Min}, {LimitA3.max}]{Environment.NewLine}";
        //        }

        //        if (!checkA4) {
        //            errorMessage += $"X: {axisPosition.A4} [{LimitA4.Min}, {LimitA4.max}]{Environment.NewLine}";
        //        }

        //        if (!checkA5) {
        //            errorMessage += $"Y: {axisPosition.A5} [{LimitA5.Min}, {LimitA5.max}]{Environment.NewLine}";
        //        }

        //        if (!checkA6) {
        //            errorMessage += $"Z: {axisPosition.A6} [{LimitA6.Min}, {LimitA6.max}]{Environment.NewLine}";
        //        }
        //    }

        //    return (result, errorMessage);
        //}

        //public (bool result, string errorMessage) CheckCorrection(E6POS correction) {
        //    string errorMessage = "";

        //    bool checkX = Math.Abs(correction.X) <= MaxCorrection.XYZ;
        //    bool checkY = Math.Abs(correction.Y) <= MaxCorrection.XYZ;
        //    bool checkZ = Math.Abs(correction.Z) <= MaxCorrection.XYZ;
        //    bool checkA = Math.Abs(correction.A) <= MaxCorrection.ABC;
        //    bool checkB = Math.Abs(correction.B) <= MaxCorrection.ABC;
        //    bool checkC = Math.Abs(correction.C) <= MaxCorrection.ABC;
        //    bool result = checkX && checkY && checkZ && checkA && checkB && checkC;

        //    if (!result) {
        //        errorMessage = $"Axis position has been exceeded:{Environment.NewLine}";

        //        if (!checkX) {
        //            errorMessage += $"X: {correction.X}{Environment.NewLine}";
        //        }

        //        if (!checkY) {
        //            errorMessage += $"Y: {correction.Y}{Environment.NewLine}";
        //        }

        //        if (!checkZ) {
        //            errorMessage += $"Z: {correction.Z}{Environment.NewLine}";
        //        }

        //        if (!checkA) {
        //            errorMessage += $"A: {correction.A}{Environment.NewLine}";
        //        }

        //        if (!checkB) {
        //            errorMessage += $"B: {correction.B}{Environment.NewLine}";
        //        }

        //        if (!checkC) {
        //            errorMessage += $"C: {correction.C}{Environment.NewLine}";
        //        }
        //    }

        //    return (result, errorMessage);
        //}

    }
}
