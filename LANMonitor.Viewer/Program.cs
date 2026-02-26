using System;
using System.Windows.Forms;

namespace LANMonitor.Viewer
{
    /// <summary>
    /// Application entry point for the LAN Screen Viewer.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.MainForm());
        }
    }
}
