using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LanOra.Security
{
    /// <summary>
    /// Thread-safe session logger. Appends timestamped entries to a dated log
    /// file in a <c>Logs/</c> subdirectory beside the application executable.
    ///
    /// All writes are dispatched to the thread-pool so the calling thread
    /// (UI or network) is never blocked by I/O.
    /// </summary>
    internal sealed class SessionLogger
    {
        private readonly string _logPath;
        private readonly object _writeLock = new object();

        public SessionLogger()
        {
            try
            {
                string exeDir  = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
                string logsDir = Path.Combine(exeDir, "Logs");
                Directory.CreateDirectory(logsDir);
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_session.txt";
                _logPath = Path.Combine(logsDir, fileName);
            }
            catch
            {
                _logPath = null;
            }
        }

        /// <summary>
        /// Appends a log entry asynchronously.  The entry is formatted as
        /// <c>[HH:mm:ss] message</c> followed by a newline.
        /// </summary>
        public void Log(string message)
        {
            if (_logPath == null) return;

            string entry = string.Format("[{0}] {1}{2}",
                DateTime.Now.ToString("HH:mm:ss"),
                message,
                Environment.NewLine);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (_writeLock)
                {
                    try { File.AppendAllText(_logPath, entry); }
                    catch { /* ignore I/O errors */ }
                }
            });
        }
    }
}
