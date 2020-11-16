using PingPong.KUKA;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PingPong.Forms {
    public partial class KUKADataPanel : UserControl {

        private const double Ts = 100;

        private readonly Stopwatch stopWatch;

        private int visibleSamples = 0;

        private int totalSamples = 0;

        private int deltaTime = 0;

        private readonly Series sx, sy, sz, sa, sb, sc;

        private readonly Series vx, vy, vz, va, vb, vc;

        private readonly Series ax, ay, az, aa, ab, ac;

        public KUKADataPanel(KUKARobot robot) {
            InitializeComponent();
            stopWatch = new Stopwatch();

            sx = new Series();
            sy = new Series();
            sz = new Series();
            sa = new Series();
            sb = new Series();
            sc = new Series();

            vx = new Series();
            vy = new Series();
            vz = new Series();
            va = new Series();
            vb = new Series();
            vc = new Series();

            ax = new Series();
            ay = new Series();
            az = new Series();
            aa = new Series();
            ab = new Series();
            ac = new Series();
        }

        private void UpdateUI(Action updateAction) {
            if (InvokeRequired) {
                Action actionWrapper = () => {
                    updateAction.Invoke();
                };

                Invoke(actionWrapper);
                return;
            }

            updateAction.Invoke();
        }

    }
}
