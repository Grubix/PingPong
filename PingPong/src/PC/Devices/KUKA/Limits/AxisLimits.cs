namespace PingPong.KUKA {
    class AxisLimits {

        public (double Min, double Max) A1 { get; }

        public (double Min, double Max) A2 { get; }

        public (double Min, double Max) A3 { get; }

        public (double Min, double Max) A4 { get; }

        public (double Min, double Max) A5 { get; }

        public (double Min, double Max) A6 { get; }

        public double MaxVelocity { get; }

        public AxisLimits(
            (double Min, double max) limitA1, 
            (double Min, double max) limitA2, 
            (double Min, double max) limitA3,
            (double Min, double max) limitA4, 
            (double Min, double max) limitA5, 
            (double Min, double max) limitA6
        ) {
            A1 = limitA1;
            A2 = limitA2;
            A3 = limitA3;
            A4 = limitA4;
            A5 = limitA5;
            A6 = limitA6;
        }

        public bool CheckAxisPosition(E6AXIS axisPosition) {
            bool checkA1 = axisPosition.A1 >= A1.Min && axisPosition.A1 <= A1.Max;
            bool checkA2 = axisPosition.A2 >= A2.Min && axisPosition.A2 <= A2.Max;
            bool checkA3 = axisPosition.A3 >= A3.Min && axisPosition.A3 <= A3.Max;
            bool checkA4 = axisPosition.A4 >= A4.Min && axisPosition.A4 <= A4.Max;
            bool checkA5 = axisPosition.A5 >= A5.Min && axisPosition.A5 <= A5.Max;
            bool checkA6 = axisPosition.A6 >= A6.Min && axisPosition.A6 <= A6.Max;

            return checkA1 && checkA2 && checkA3 && checkA4 && checkA5 && checkA6;
        }
    }
}
