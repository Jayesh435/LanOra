using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LanOra.Utilities;

namespace LanOra
{
    /// <summary>
    /// Application entry point. Opens the role-selection screen and performs
    /// a background auto-update check when the check interval has elapsed.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Kick off the update check on a background thread so it never
            // blocks the UI thread or slows application startup.
            ThreadPool.QueueUserWorkItem(_ => CheckForUpdates());

            Application.Run(new Forms.RoleSelectForm());
        }

        /// <summary>
        /// Checks the remote version manifest and, if a newer version is
        /// available, shows the update dialog on the UI thread.
        /// </summary>
        private static void CheckForUpdates()
        {
            try
            {
                if (!UpdateChecker.IsCheckDue())
                    return;

                UpdateLogger.Log("Program: Starting background update check.");

                VersionInfo remote = UpdateChecker.FetchVersionInfo();
                UpdateChecker.RecordCheckTime();

                if (!UpdateChecker.IsUpdateAvailable(remote))
                    return;

                // Marshal the dialog onto the UI thread via the role-select
                // form, which is the root form for the entire application.
                // Use OfType to avoid IndexOutOfRangeException if the form
                // collection changes before the check completes.
                Forms.RoleSelectForm mainForm =
                    Application.OpenForms.OfType<Forms.RoleSelectForm>()
                               .FirstOrDefault();

                if (mainForm == null || mainForm.IsDisposed)
                    return;

                mainForm.BeginInvoke((Action)(() =>
                {
                    using (var dlg = new Forms.UpdateDialog(remote))
                        dlg.ShowDialog(mainForm);
                }));
            }
            catch (Exception ex)
            {
                UpdateLogger.LogException("Program: Update check failed", ex);
            }
        }
    }
}
