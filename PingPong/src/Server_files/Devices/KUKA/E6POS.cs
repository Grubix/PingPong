namespace PingPong.Devices {
    public class E6POS {

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public E6POS() {

        }

        public E6POS(double X, double Y, double Z, double A, double B, double C) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.A = A;
            this.B = B;
            this.C = C;
        }

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

        public static bool operator == (E6POS pos1, E6POS pos2) {
            return pos1.X == pos2.X &&
                pos1.Y == pos2.Y &&
                pos1.Z == pos2.Z &&
                pos1.A == pos2.A &&
                pos1.B == pos2.B &&
                pos1.C == pos2.C;
        }

        public static bool operator != (E6POS pos1, E6POS pos2) {
            return pos1.X != pos2.X ||
                pos1.Y != pos2.Y ||
                pos1.Z != pos2.Z ||
                pos1.A != pos2.A ||
                pos1.B != pos2.B ||
                pos1.C != pos2.C;
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

    }
}
