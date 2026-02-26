using System;
using System.Windows.Forms;

namespace LanOra
{
    /// <summary>
    /// Application entry point. Opens the role-selection screen.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.RoleSelectForm());
        }
    }
}
