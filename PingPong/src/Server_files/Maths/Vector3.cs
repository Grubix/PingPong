using System;

namespace PingPong.Maths {
    class Vector3 {

        private double[] components;

        public double X {
            get {
                return components[0];
            }
            set {
                components[0] = value;
            }
        }

        public double Y {
            get {
                return components[1];
            }
            set {
                components[1] = value;
            }
        }

        public double Z {
            get {
                return components[2];
            }
            set {
                components[2] = value;
            }
        }

        public double this[uint i] {
            get {
                if (i >= 2) {
                    throw new ArgumentException($"Index {i} out of bounds");
                }

                return components[i];
            }
            set {
                if (i >= 2) {
                    throw new ArgumentException($"Index {i} out of bounds");
                }

                components[i] = value;
            }
        }

        public Vector3() {
            components = new double[] {
                0, 0, 0
            };
        }

        public Vector3(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

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

        public static Vector3 operator *(Vector3 vec, double value) {
            return new Vector3() {
                X = vec.X * value,
                Y = vec.Y * value,
                Z = vec.Z * value
            };
        }

        public static Vector3 operator *(double value, Vector3 vec) {
            return vec * value;
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
