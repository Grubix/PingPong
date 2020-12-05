using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Views {
    public partial class CollisionTest : Form {

        public CollisionTest() {
            InitializeComponent();

            plotBtn.Click += (s, e) => Plot();
        }

        private void Plot() {
            chart.Series[0].Points.Clear();
            chart.Series[1].Points.Clear();
            chart.Series[2].Points.Clear();
            chart.Annotations.Clear();

            double z0 = double.Parse(textz0.Text);
            double v0 = double.Parse(textv0.Text); ;
            double a0 = 0.0;

            double z1 = double.Parse(textz1.Text); ; // Miejsce zderzenia na zecie
            double v1 = double.Parse(textv1.Text); ; // Predkosc na zecie w momencie zderzenia
            double a1 = 0.0;

            double z2 = double.Parse(textz2.Text); ; // Gdzie ma dojechac po zderzeniu (wg mnie ma sie na zetce cofnac)
            double v2 = double.Parse(textv2.Text); ;

            double T1 = double.Parse(textt1.Text); ; // Czas do zderzenia
            double T2 = double.Parse(textt2.Text); ; // Czas wyhamowywania

            double[] coeffs1 = GetCoeffs(z0, v0, a0, z1, v1, T1);
            double[] coeffs2 = GetCoeffs(z1, v1, a1, z2, v2, T2);

            double k0 = coeffs1[0];
            double k1 = coeffs1[1];
            double k2 = coeffs1[2];
            double k3 = coeffs1[3];
            double k4 = coeffs1[4];
            double k5 = coeffs1[5];

            double i0 = coeffs2[0];
            double i1 = coeffs2[1];
            double i2 = coeffs2[2];
            double i3 = coeffs2[3];
            double i4 = coeffs2[4];
            double i5 = coeffs2[5];

            for (double t = 0.0; t <= T1; t += 0.01) {
                double t1 = t;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                double x = k5 * t5 + k4 * t4 + k3 * t3 + k2 * t2 + k1 * t1 + k0;
                double v = 5.0 * k5 * t4 + 4.0 * k4 * t3 + 3.0 * k3 * t2 + 2.0 * k2 * t1 + k1;
                double a = 20.0 * k5 * t3 + 12.0 * k4 * t2 + 6.0 * k3 * t1 + 2.0 * k2;

                chart.Series[0].Points.AddXY(t1, x);
                chart.Series[1].Points.AddXY(t1, v);
                //chart.Series[2].Points.AddXY(t1, a);
            }

            for (double t = 0.0; t <= T2; t += 0.01) {
                double t1 = t;
                double t2 = t1 * t1;
                double t3 = t1 * t2;
                double t4 = t1 * t3;
                double t5 = t1 * t4;

                double x = i5 * t5 + i4 * t4 + i3 * t3 + i2 * t2 + i1 * t1 + i0;
                double v = 5.0 * i5 * t4 + 4.0 * i4 * t3 + 3.0 * i3 * t2 + 2.0 * i2 * t1 + i1;
                double a = 20.0 * i5 * t3 + 12.0 * i4 * t2 + 6.0 * i3 * t1 + 2.0 * i2;

                chart.Series[0].Points.AddXY(t1 + T1, x);
                chart.Series[1].Points.AddXY(t1 + T1, v);
                //chart1.Series[2].Points.AddXY(t1 + T1, a);
            }

            VerticalLineAnnotation ann = new VerticalLineAnnotation();
            ann.X = T1;
            ann.AxisX = chart.ChartAreas[0].AxisX;
            ann.IsInfinitive = true;
            ann.LineColor = Color.Red;
            ann.LineDashStyle = ChartDashStyle.DashDot;

            chart.Annotations.Add(ann);
        }

        private double[] GetCoeffs(double z0, double v0, double a0, double z1, double v1, double T) {
            double T1 = T;
            double T2 = T1 * T1;
            double T3 = T1 * T2;
            double T4 = T1 * T3;
            double T5 = T1 * T4;

            double k0 = z0;
            double k1 = v0;
            double k2 = a0 / 2.0;
            double k3 = 1.0 / (2.0 * T3) * (-3.0 * T2 * a0 - 12.0 * T1 * v0 - 8.0 * T1 * v1 + 20.0 * (z1 - z0));
            double k4 = 1.0 / (2.0 * T4) * (3.0 * T2 * a0 + 16.0 * T1 * v0 + 14.0 * T1 * v1 - 30.0 * (z1 - z0));
            double k5 = 1.0 / (2.0 * T5) * (-T2 * a0 - 6.0 * T1 * (v0 + v1) + 12.0 * (z1 - z0));

            return new double[] {
                k0, k1, k2, k3, k4, k5
            };
        }

    }
}
