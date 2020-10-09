using System;
using System.Windows.Forms;

namespace PingPong {
    /// <summary>
    /// Application entry point
    /// </summary>
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }
        
    }
}
