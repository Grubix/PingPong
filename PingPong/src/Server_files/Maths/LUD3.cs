namespace PingPong.Maths {
    class LUD3 {

        public Matrix3 L { get; private set; }

        public Matrix3 U { get; private set; }

        public LUD3(Matrix3 input) {
            L = new Matrix3();
            U = new Matrix3();

            for (int i = 0; i < 3; i++) {
                // U matrix
                for (int k = i; k < 3; k++) {
                    double sum1 = 0.0;

                    for (int j = 0; j < i; j++) {
                        sum1 += L[i, j] * U[j, k];
                    }

                    U[i, k] = input[i, k] - sum1;
                }

                // L matrix
                for (int k = i; k < 3; k++) {
                    if (i == k) {
                        L[i, i] = 1.0;
                    } else {
                        double sum2 = 0.0;

                        for (int j = 0; j < i; j++) {
                            sum2 += L[k, j] * U[j, i];
                        }

                        L[k, i] = (input[k, i] - sum2) / U[i, i];
                    }
                }
            }
        }

    }
}
