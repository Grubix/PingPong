namespace PingPong.KUKA {
    public class AxisLimits {

        public (double Min, double Max) A1 { get; }

        public (double Min, double Max) A2 { get; }

        public (double Min, double Max) A3 { get; }

        public (double Min, double Max) A4 { get; }

        public (double Min, double Max) A5 { get; }

        public (double Min, double Max) A6 { get; }

        public double MaxVelocity { get; }

        public AxisLimits(
            (double Min, double Max) A1, 
            (double Min, double Max) A2, 
            (double Min, double Max) A3,
            (double Min, double Max) A4, 
            (double Min, double Max) A5, 
            (double Min, double Max) A6
        ) {
            this.A1 = A1;
            this.A2 = A2;
            this.A3 = A3;
            this.A4 = A4;
            this.A5 = A5;
            this.A6 = A6;
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
