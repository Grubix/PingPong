using System;

namespace PingPong.Maths {
    class Matrix3 {

        private readonly double[,] matrix;

        public double this[int i, int j] {
            get {
                return matrix[i, j];
            }
            set {
                matrix[i, j] = value;
            }
        }

        public Matrix3() {
            matrix = new double[,] {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };
        }

        public Matrix3(double[,] matrix) {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != 3 || cols != 3) {
                throw new ArgumentException($"3x3 matrix expected, get {rows}x{cols}");
            }

            this.matrix = matrix.Clone() as double[,];
        }

        public double Determinant() {
            double m11 = matrix[0, 0];
            double m12 = matrix[0, 1];
            double m13 = matrix[0, 2];

            double m21 = matrix[1, 0];
            double m22 = matrix[1, 1];
            double m23 = matrix[1, 2];

            double m31 = matrix[2, 0];
            double m32 = matrix[2, 1];
            double m33 = matrix[2, 2];

            return
                m11 * (m22 * m33 - m23 * m32)
                - m12 * (m21 * m33 - m23 * m31)
                - m13 * (m22 * m31 - m21 * m32);
        }

        public Matrix3 Transpose() {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = matrix[j, i];
                }
            }

            return result;
        }

        public Matrix3 Inverse() {
            double determinant = Determinant();

            if (determinant == 0) {
                throw new InvalidOperationException("Determinant of the matrix equals zero, matrix is not invertible");
            }

            Matrix3 inverse = new Matrix3();

            double m11 = matrix[0, 0];
            double m12 = matrix[0, 1];
            double m13 = matrix[0, 2];

            double m21 = matrix[1, 0];
            double m22 = matrix[1, 1];
            double m23 = matrix[1, 2];

            double m31 = matrix[2, 0];
            double m32 = matrix[2, 1];
            double m33 = matrix[2, 2];

            inverse[0, 0] = (m22 * m33 - m23 * m32) / determinant;
            inverse[0, 1] = -(m12 * m33 - m13 * m32) / determinant;
            inverse[0, 2] = (m12 * m23 - m13 * m22) / determinant;

            inverse[1, 0] = -(m21 * m33 - m23 * m31) / determinant;
            inverse[1, 1] = (m11 * m33 - m13 * m31) / determinant;
            inverse[1, 2] = -(m11 * m23 - m13 * m21) / determinant;

            inverse[2, 0] = (m21 * m32 - m22 * m31) / determinant;
            inverse[2, 1] = -(m11 * m32 - m12 * m31) / determinant;
            inverse[2, 2] = (m11 * m22 - m12 * m21) / determinant;

            return inverse;
        }

        public LUD3 LUD() {
            return new LUD3(this);
        }

        public SVD3 SVD(double tolerance = 0.00001) {
            return new SVD3(this, tolerance);
        }

        public override string ToString() {
            string result = "";

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += $"{matrix[i, j]:F3}\t";
                }
                result += "\n";
            }

            return result;
        }

        public static Matrix3 Identity() {
            Matrix3 result = new Matrix3();

            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = 1;

            return result;
        }

        public static Matrix3 operator +(Matrix3 mat1, Matrix3 mat2) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = mat1[i, j] + mat2[i, j];
                }
            }

            return result;
        }

        public static Matrix3 operator +(Matrix3 mat, double value) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = mat[i, j];
                }
            }

            result[0, 0] += value;
            result[1, 1] += value;
            result[2, 2] += value;

            return result;
        }

        public static Matrix3 operator +(double value, Matrix3 mat) {
            return mat + value;
        }

        public static Matrix3 operator -(Matrix3 mat) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = -mat[i, j];
                }
            }

            return result;
        }

        public static Matrix3 operator -(Matrix3 mat1, Matrix3 mat2) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = mat1[i, j] - mat2[i, j];
                }
            }

            return result;
        }

        public static Matrix3 operator -(Matrix3 mat, double value) {
            return mat + (-value);
        }

        public static Matrix3 operator -(double value, Matrix3 mat) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = -mat[i, j];
                }
            }

            result[0, 0] += value;
            result[1, 1] += value;
            result[2, 2] += value;

            return result;
        }

        public static Matrix3 operator *(Matrix3 mat1, Matrix3 mat2) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    double cellValue = 0;

                    for (int k = 0; k < 3; k++) {
                        cellValue += mat1[i, k] * mat2[k, j];
                    }

                    result[i, j] = cellValue;
                }
            }

            return result;
        }

        public static Vector3 operator *(Matrix3 mat, Vector3 vec) {
            return new Vector3() {
                X = mat[0, 0] * vec.X + mat[0, 1] * vec.Y + mat[0, 2] * vec.Z,
                Y = mat[1, 0] * vec.X + mat[1, 1] * vec.Y + mat[1, 2] * vec.Z,
                Z = mat[2, 0] * vec.X + mat[2, 1] * vec.Y + mat[2, 2] * vec.Z
            };
        }

        public static Matrix3 operator *(Matrix3 mat, double value) {
            Matrix3 result = new Matrix3();

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result[i, j] = mat[i, j] * value;
                }
            }

            return result;
        }

        public static Matrix3 operator *(double value, Matrix3 mat) {
            return mat * value;
        }

        public static Matrix3 operator /(Matrix3 mat1, Matrix3 mat2) {
            return mat1 * mat2.Inverse();
        }

        public static Matrix3 operator /(Matrix3 mat, double divider) {
            return mat * 1 / (divider);
        }

    }
}