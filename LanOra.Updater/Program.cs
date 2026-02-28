using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LanOra.Updater
{
    /// <summary>
    /// LanOra Auto-Updater
    ///
    /// Invoked by LanOra.exe after a verified update zip has been downloaded.
    /// The updater runs as a separate process so it can replace its parent's
    /// files while the parent is stopped.
    ///
    /// Command-line arguments (all required, in order):
    ///   [0] lanoraProcessId  – PID of the running LanOra process to wait for
    ///   [1] zipPath          – full path to the downloaded (verified) zip
    ///   [2] installDir       – directory where LanOra is installed
    ///   [3] mainExePath      – full path to LanOra.exe (used to restart)
    ///
    /// Exit codes:
    ///   0 – update applied successfully and LanOra restarted
    ///   1 – update failed (rollback attempted)
    ///   2 – argument validation failed
    /// </summary>
    static class Program
    {
        // ------------------------------------------------------------------
        // Entry point
        // ------------------------------------------------------------------

        [STAThread]
        static int Main(string[] args)
        {
            Log("=== LanOra Updater started ===");
            Log("Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            Log("Args: " + string.Join(" | ", args));

            if (args.Length < 4)
            {
                Log("ERROR: Insufficient arguments. Expected: <pid> <zipPath> <installDir> <mainExe>");
                ShowError("Updater was launched with incorrect arguments.\n\nUpdate aborted.");
                return 2;
            }

            int    lanoraPid  = 0;
            string zipPath    = args[1];
            string installDir = args[2];
            string mainExe    = args[3];

            if (!int.TryParse(args[0], out lanoraPid))
            {
                Log("ERROR: Invalid LanOra process ID: " + args[0]);
                ShowError("Invalid process ID argument. Update aborted.");
                return 2;
            }

            if (!File.Exists(zipPath))
            {
                Log("ERROR: Zip file not found: " + zipPath);
                ShowError("Update package not found:\n" + zipPath + "\n\nUpdate aborted.");
                return 2;
            }

            if (!Directory.Exists(installDir))
            {
                Log("ERROR: Install directory not found: " + installDir);
                ShowError("Installation directory not found:\n" + installDir + "\n\nUpdate aborted.");
                return 2;
            }

            // ------------------------------------------------------------------
            // Step 1: Wait for LanOra to exit
            // ------------------------------------------------------------------
            Log("Waiting for LanOra process (PID " + lanoraPid + ") to exit…");
            if (!WaitForProcess(lanoraPid, timeoutMs: 30000))
            {
                Log("WARNING: LanOra did not exit within 30 s. Attempting forceful termination.");
                KillProcess(lanoraPid);
                Thread.Sleep(2000);
            }
            Log("LanOra process has exited.");

            // ------------------------------------------------------------------
            // Step 2: Create backup of current installation
            // ------------------------------------------------------------------
            string backupDir = Path.Combine(installDir, "Backup",
                DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Log("Creating backup at: " + backupDir);
            try
            {
                CopyDirectory(installDir, backupDir,
                    skip: new[] { "Backup", "UpdateTemp" });
                Log("Backup created successfully.");
            }
            catch (Exception ex)
            {
                Log("ERROR: Backup failed: " + ex.Message);
                ShowError("Failed to create backup:\n" + ex.Message + "\n\nUpdate aborted.");
                return 1;
            }

            // ------------------------------------------------------------------
            // Step 3: Extract update to a temporary staging area, then replace
            // ------------------------------------------------------------------
            string stagingDir = Path.Combine(installDir, "UpdateTemp", "staging");

            try
            {
                Log("Extracting zip to staging: " + stagingDir);
                if (Directory.Exists(stagingDir))
                    Directory.Delete(stagingDir, recursive: true);
                Directory.CreateDirectory(stagingDir);

                ZipFile.ExtractToDirectory(zipPath, stagingDir);
                Log("Extraction complete.");

                // ------------------------------------------------------------------
                // Step 4: Replace files from staging into install directory
                // ------------------------------------------------------------------
                Log("Replacing files in: " + installDir);
                CopyDirectory(stagingDir, installDir, skip: null);
                Log("File replacement complete.");
            }
            catch (Exception ex)
            {
                Log("ERROR during update: " + ex.Message);
                Log("Attempting rollback from: " + backupDir);

                bool rolledBack = TryRollback(installDir, backupDir);
                string msg = rolledBack
                    ? "Update failed. Your previous installation has been restored.\n\nError: " + ex.Message
                    : "Update failed AND rollback failed.\n\nPlease reinstall LanOra manually.\n\nError: " + ex.Message;

                ShowError(msg);
                return 1;
            }

            // ------------------------------------------------------------------
            // Step 5: Clean up temporary files
            // ------------------------------------------------------------------
            Log("Cleaning up temporary files.");
            SafeDeleteDirectory(Path.Combine(installDir, "UpdateTemp"));
            SafeDeleteFile(zipPath);
            Log("Cleanup complete.");

            // ------------------------------------------------------------------
            // Step 6: Restart LanOra
            // ------------------------------------------------------------------
            Log("Restarting LanOra: " + mainExe);
            try
            {
                if (File.Exists(mainExe))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName        = mainExe,
                        UseShellExecute = true,
                        WorkingDirectory = installDir
                    });
                    Log("LanOra restarted successfully.");
                }
                else
                {
                    Log("WARNING: LanOra.exe not found at expected path: " + mainExe);
                }
            }
            catch (Exception ex)
            {
                Log("WARNING: Failed to restart LanOra: " + ex.Message);
                // Non-fatal – the update was successful even if the restart fails.
            }

            Log("=== LanOra Updater finished successfully ===");
            return 0;
        }

        // ------------------------------------------------------------------
        // Process helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Waits for the process with the given PID to exit, up to
        /// <paramref name="timeoutMs"/> milliseconds.  Returns true if the
        /// process has exited (or was not found), false on timeout.
        /// </summary>
        private static bool WaitForProcess(int pid, int timeoutMs)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                return p.WaitForExit(timeoutMs);
            }
            catch (ArgumentException)
            {
                // Process not found – already exited.
                return true;
            }
            catch (Exception ex)
            {
                Log("WaitForProcess exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>Attempts to kill the process with the given PID.</summary>
        private static void KillProcess(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                p.Kill();
                Log("Process " + pid + " killed.");
            }
            catch (Exception ex)
            {
                Log("KillProcess exception (may be already exited): " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // File / directory helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Recursively copies all files and sub-directories from
        /// <paramref name="source"/> to <paramref name="destination"/>,
        /// optionally skipping top-level directory names listed in
        /// <paramref name="skip"/>.
        /// Creates the destination directory if it does not exist.
        /// </summary>
        private static void CopyDirectory(string source, string destination,
                                          string[] skip)
        {
            Directory.CreateDirectory(destination);

            // Copy files.
            foreach (string srcFile in Directory.GetFiles(source))
            {
                string destFile = Path.Combine(destination,
                                               Path.GetFileName(srcFile));
                File.Copy(srcFile, destFile, overwrite: true);
            }

            // Recurse into sub-directories.
            foreach (string srcDir in Directory.GetDirectories(source))
            {
                string dirName = Path.GetFileName(srcDir);

                // Skip directories that must not be touched during copy.
                if (skip != null)
                {
                    bool shouldSkip = false;
                    foreach (string s in skip)
                        if (string.Equals(dirName, s, StringComparison.OrdinalIgnoreCase))
                        { shouldSkip = true; break; }
                    if (shouldSkip) continue;
                }

                string destDir = Path.Combine(destination, dirName);
                CopyDirectory(srcDir, destDir, skip: null);
            }
        }

        /// <summary>
        /// Attempts to restore the install directory from the backup.
        /// Returns true on success.
        /// </summary>
        private static bool TryRollback(string installDir, string backupDir)
        {
            if (!Directory.Exists(backupDir))
            {
                Log("Rollback: backup directory not found.");
                return false;
            }

            try
            {
                Log("Rollback: copying backup files back to install directory.");
                CopyDirectory(backupDir, installDir,
                    skip: new[] { "Backup", "UpdateTemp" });
                Log("Rollback: complete.");
                return true;
            }
            catch (Exception ex)
            {
                Log("Rollback: FAILED – " + ex.Message);
                return false;
            }
        }

        private static void SafeDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive: true);
            }
            catch (Exception ex)
            {
                Log("SafeDeleteDirectory failed for '" + path + "': " + ex.Message);
            }
        }

        private static void SafeDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                Log("SafeDeleteFile failed for '" + path + "': " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // Logging
        // ------------------------------------------------------------------

        private static readonly string _logPath;
        private static readonly object _logLock = new object();

        static Program()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "LanOra", "Logs");
            try { Directory.CreateDirectory(dir); } catch { /* ignore */ }
            _logPath = Path.Combine(dir, "LanOraUpdate.log");
        }

        private static void Log(string message)
        {
            string line = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] [Updater] {1}",
                                        DateTime.Now, message);
            Console.WriteLine(line);

            lock (_logLock)
            {
                try
                {
                    File.AppendAllText(_logPath, line + Environment.NewLine, Encoding.UTF8);
                }
                catch { /* ignore write failures */ }
            }
        }

        // ------------------------------------------------------------------
        // UI helpers
        // ------------------------------------------------------------------

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "LanOra Updater",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
