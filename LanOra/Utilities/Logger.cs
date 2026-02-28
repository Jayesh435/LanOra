using System;
using System.IO;
using System.Text;

namespace LanOra.Utilities
{
    /// <summary>
    /// Thread-safe diagnostic logger. Appends timestamped lines to LanOra.log
    /// in the executable directory. Log failures are silently swallowed so that
    /// logging never crashes the application.
    /// </summary>
    internal static class Logger
    {
        private static readonly string _logPath;
        private static readonly object _lock = new object();

        static Logger()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            _logPath   = Path.Combine(dir, "LanOra.log");
        }

        /// <summary>Appends a timestamped message to LanOra.log.</summary>
        public static void Log(string message)
        {
            string line = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, message);
            lock (_lock)
            {
                try { File.AppendAllText(_logPath, line + Environment.NewLine, Encoding.UTF8); }
                catch { /* ignore write failures */ }
            }
        }

        /// <summary>Logs current GC-managed heap size in megabytes.</summary>
        public static void LogMemory()
        {
            long bytes = GC.GetTotalMemory(false);
            Log(string.Format("Memory usage: {0:F1} MB (GC managed heap)", bytes / (1024.0 * 1024.0)));
        }
    }
}
