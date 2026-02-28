using System;
using System.IO;
using System.Text;

namespace LanOra.Utilities
{
    /// <summary>
    /// Thread-safe logger dedicated to the auto-update subsystem.
    /// Writes timestamped entries to:
    ///   %ProgramData%\LanOra\Logs\LanOraUpdate.log
    /// Log failures are silently swallowed so that the updater never crashes
    /// the application due to a write error.
    /// </summary>
    internal static class UpdateLogger
    {
        private static readonly string _logPath;
        private static readonly object _lock = new object();

        static UpdateLogger()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "LanOra", "Logs");

            try { Directory.CreateDirectory(dir); } catch { /* ignore */ }

            _logPath = Path.Combine(dir, "LanOraUpdate.log");
        }

        /// <summary>Returns the full path to the update log file.</summary>
        public static string LogPath { get { return _logPath; } }

        /// <summary>Appends a timestamped message to LanOraUpdate.log.</summary>
        public static void Log(string message)
        {
            string line = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}",
                                        DateTime.Now, message);
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_logPath, line + Environment.NewLine, Encoding.UTF8);
                }
                catch { /* ignore write failures */ }
            }
        }

        /// <summary>Logs an exception with a descriptive prefix.</summary>
        public static void LogException(string context, Exception ex)
        {
            Log(string.Format("{0}: [{1}] {2}", context, ex.GetType().Name, ex.Message));
        }
    }
}
