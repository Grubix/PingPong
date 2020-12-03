using PingPong.Views;
using System;
using System.IO;
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

            Directory.CreateDirectory("transformations");
            Directory.CreateDirectory("logs");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

    }
}
