namespace PingPong.Maths {
    class Vector3 {

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double Dot(Vector3 vec) {
            return X * vec.X + Y * vec.Y + Z * vec.Z;
        }

        public Vector3 Cross(Vector3 vec) {
            return new Vector3() {
                X = Y * vec.Z - Z * vec.Y,
                Y = X * vec.Z - Z * vec.X,
                Z = X * vec.Y - Y * vec.X
            };
        }

        public double Norm() {
            return System.Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3 Normalize() {
            return this / Norm();
        }

        public static Vector3 operator +(Vector3 vec1, Vector3 vec2) {
            return new Vector3() {
                X = vec1.X + vec2.X,
                Y = vec1.Y + vec2.Y,
                Z = vec1.Z + vec2.Z
            };
        }

        public static Vector3 operator -(Vector3 vec1, Vector3 vec2) {
            return new Vector3() {
                X = vec1.X - vec2.X,
                Y = vec1.Y - vec2.Y,
                Z = vec1.Z - vec2.Z
            };
        }

        public static Vector3 operator -(Vector3 vec) {
            return new Vector3() {
                X = -vec.X,
                Y = -vec.Y,
                Z = -vec.Z
            };
        }

        public static Vector3 operator *(Vector3 vec, double multiplier) {
            return new Vector3() {
                X = vec.X * multiplier,
                Y = vec.Y * multiplier,
                Z = vec.Z * multiplier
            };
        }

        public static Vector3 operator *(Vector3 vec, Matrix3 mat) {
            return new Vector3() {
                X = mat[0, 0] * vec.X + mat[1, 0] * vec.Y + mat[2, 0] * vec.Z,
                Y = mat[0, 1] * vec.X + mat[1, 1] * vec.Y + mat[2, 1] * vec.Z,
                Z = mat[0, 2] * vec.X + mat[1, 2] * vec.Y + mat[2, 2] * vec.Z
            };
        }

        public static Vector3 operator /(Vector3 vec, double divider) {
            return vec * (1 / divider);
        }

    }
}
