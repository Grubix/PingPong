namespace PingPong.Devices {
    public class E6POS {

        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public double Z { get; set; } = 0.0;
        public double A { get; set; } = 0.0;
        public double B { get; set; } = 0.0;
        public double C { get; set; } = 0.0;

        public static E6POS operator + (E6POS pos1, E6POS pos2) {
            return new E6POS() {
                X = pos1.X + pos2.X,
                Y = pos1.Y + pos2.Y,
                Z = pos1.Z + pos2.Z,
                A = pos1.A + pos2.A,
                B = pos1.B + pos2.B,
                C = pos1.C + pos2.C
            };
        }

        public static E6POS operator - (E6POS pos1, E6POS pos2) {
            return new E6POS() {
                X = pos1.X - pos2.X,
                Y = pos1.Y - pos2.Y,
                Z = pos1.Z - pos2.Z,
                A = pos1.A - pos2.A,
                B = pos1.B - pos2.B,
                C = pos1.C - pos2.C
            };
        }

    }
}
