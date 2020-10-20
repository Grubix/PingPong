using PingPong.Forms;
using System;
using System.Windows.Forms;

namespace PingPong {
    /// <summary>
    /// Application entry point
    /// </summary>
    static class Program {

        [STAThread]
        static void Main() {
            if (Environment.OSVersion.Version.Major >= 6) {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

    }
}
