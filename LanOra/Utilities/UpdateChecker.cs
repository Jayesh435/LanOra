using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LanOra.Utilities
{
    /// <summary>
    /// Holds the information parsed from the remote version.json manifest.
    /// </summary>
    internal sealed class VersionInfo
    {
        public Version Version     { get; set; }
        public string  DownloadUrl { get; set; }
        public string  Sha256      { get; set; }
        public bool    Mandatory   { get; set; }
        public string  Channel     { get; set; }
        public string  ReleaseNotes { get; set; }
    }

    /// <summary>
    /// Delegate used to report download progress (0–100 %).
    /// </summary>
    internal delegate void DownloadProgressCallback(int percent, long bytesReceived, long totalBytes);

    /// <summary>
    /// Manages the complete auto-update lifecycle for LanOra:
    ///   1. Fetch and parse the remote version.json manifest.
    ///   2. Compare with the locally installed version.
    ///   3. Download the update zip with SHA-256 integrity verification.
    ///   4. Launch Updater.exe and exit the main application.
    ///
    /// Win-7 safe: uses only WebClient, System.IO, and BCL cryptography.
    /// No external NuGet packages required.
    /// </summary>
    internal static class UpdateChecker
    {
        // ------------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------------

        /// <summary>
        /// URL of the remote version manifest.
        /// Must use HTTPS to prevent man-in-the-middle substitution of the
        /// update payload.
        /// </summary>
        public static string VersionUrl { get; set; } =
            "https://yourdomain.com/lanora/version.json";

        /// <summary>
        /// Minimum interval between automatic background checks.
        /// Stored as a Unix-epoch timestamp in a small state file so that the
        /// check survives application restarts.
        /// </summary>
        public static TimeSpan CheckInterval { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Update channel the client subscribes to ("stable" or "beta").
        /// Compared case-insensitively against the channel field in version.json.
        /// </summary>
        public static string Channel { get; set; } = "stable";

        // ------------------------------------------------------------------
        // State file path
        // ------------------------------------------------------------------

        private static readonly string _stateFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "LanOra", "last_update_check.txt");

        // ------------------------------------------------------------------
        // Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Returns the version of the currently running LanOra assembly.
        /// </summary>
        public static Version LocalVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Returns true when enough time has elapsed since the last check,
        /// allowing callers to skip redundant network requests.
        /// </summary>
        public static bool IsCheckDue()
        {
            try
            {
                if (!File.Exists(_stateFile))
                    return true;

                string text = File.ReadAllText(_stateFile, Encoding.UTF8).Trim();
                long   ticks;
                if (!long.TryParse(text, out ticks))
                    return true;

                DateTime last    = new DateTime(ticks, DateTimeKind.Utc);
                bool     isDue   = (DateTime.UtcNow - last) >= CheckInterval;
                return isDue;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Records the current time as the "last checked" timestamp.
        /// Called after a successful version-manifest fetch.
        /// </summary>
        public static void RecordCheckTime()
        {
            try
            {
                string dir = Path.GetDirectoryName(_stateFile);
                Directory.CreateDirectory(dir);
                File.WriteAllText(_stateFile,
                                  DateTime.UtcNow.Ticks.ToString(),
                                  Encoding.UTF8);
            }
            catch { /* non-critical */ }
        }

        /// <summary>
        /// Downloads and parses the remote version.json manifest.
        /// Returns null if the manifest cannot be fetched or parsed.
        /// </summary>
        public static VersionInfo FetchVersionInfo()
        {
            if (string.IsNullOrEmpty(VersionUrl))
            {
                UpdateLogger.Log("UpdateChecker: VersionUrl is not configured.");
                return null;
            }

            // Enforce HTTPS to prevent downgrade/MITM attacks.
            if (!VersionUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                UpdateLogger.Log("UpdateChecker: VersionUrl must use HTTPS. Aborting check.");
                return null;
            }

            UpdateLogger.Log("UpdateChecker: Fetching version manifest from " + VersionUrl);

            try
            {
                string json;
                using (WebClient wc = CreateWebClient())
                {
                    json = wc.DownloadString(VersionUrl);
                }

                VersionInfo info = ParseVersionJson(json);
                UpdateLogger.Log(string.Format(
                    "UpdateChecker: Remote version = {0} (mandatory={1}, channel={2})",
                    info.Version, info.Mandatory, info.Channel));

                return info;
            }
            catch (Exception ex)
            {
                UpdateLogger.LogException("UpdateChecker: Failed to fetch version manifest", ex);
                return null;
            }
        }

        /// <summary>
        /// Returns true when <paramref name="remote"/> is newer than the
        /// currently installed version.  Prevents downgrade attacks by
        /// requiring a strictly greater version.
        /// </summary>
        public static bool IsUpdateAvailable(VersionInfo remote)
        {
            if (remote == null || remote.Version == null)
                return false;

            // Channel filter: only offer updates from the subscribed channel.
            if (!string.IsNullOrEmpty(remote.Channel) &&
                !string.Equals(remote.Channel, Channel, StringComparison.OrdinalIgnoreCase))
            {
                UpdateLogger.Log(string.Format(
                    "UpdateChecker: Ignoring channel '{0}' (subscribed to '{1}').",
                    remote.Channel, Channel));
                return false;
            }

            bool available = remote.Version > LocalVersion;
            UpdateLogger.Log(string.Format(
                "UpdateChecker: Local={0}, Remote={1} → update available={2}",
                LocalVersion, remote.Version, available));

            return available;
        }

        /// <summary>
        /// Downloads the update zip to a temporary file, verifying its
        /// SHA-256 hash against the value declared in the manifest.
        /// Returns the local path to the verified zip, or null on failure.
        /// </summary>
        /// <param name="info">Remote version manifest (from FetchVersionInfo).</param>
        /// <param name="progress">Optional progress callback (may be null).</param>
        /// <param name="cancellationToken">
        ///   Checked periodically; set to CancellationToken.None to disable cancellation.
        /// </param>
        public static string DownloadUpdate(VersionInfo info,
                                            DownloadProgressCallback progress,
                                            CancellationToken cancellationToken)
        {
            if (info == null) throw new ArgumentNullException("info");

            // Enforce HTTPS for the download URL as well.
            if (string.IsNullOrEmpty(info.DownloadUrl) ||
                !info.DownloadUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                UpdateLogger.Log("UpdateChecker: DownloadUrl is empty or not HTTPS. Aborting download.");
                return null;
            }

            string tempDir  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateTemp");
            Directory.CreateDirectory(tempDir);

            string tempZip  = Path.Combine(tempDir, "update_download.tmp");
            string finalZip = Path.Combine(tempDir,
                string.Format("LanOra_{0}.zip", info.Version));

            UpdateLogger.Log("UpdateChecker: Starting download from " + info.DownloadUrl);
            UpdateLogger.Log("UpdateChecker: Destination = " + tempZip);

            try
            {
                using (WebClient wc = CreateWebClient())
                {
                    // Track the last logged percentage to throttle log output
                    // to one entry per 10 % increment.
                    int lastLoggedPercent = -1;

                    wc.DownloadProgressChanged += (s, e) =>
                    {
                        // Log only at 0%, 10%, 20%, … 100% boundaries.
                        int bucket = (e.ProgressPercentage / 10) * 10;
                        if (bucket > lastLoggedPercent)
                        {
                            lastLoggedPercent = bucket;
                            UpdateLogger.Log(string.Format(
                                "UpdateChecker: Download progress {0}% ({1}/{2} bytes)",
                                e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive));
                        }

                        if (progress != null)
                            progress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
                    };

                    // Synchronous wrapper around the async download so we can
                    // poll the cancellation token without blocking the UI.
                    using (ManualResetEventSlim done = new ManualResetEventSlim(false))
                    {
                        Exception downloadError = null;

                        wc.DownloadFileCompleted += (s, e) =>
                        {
                            downloadError = e.Error;
                            done.Set();
                        };

                        wc.DownloadFileAsync(new Uri(info.DownloadUrl), tempZip);

                        // Poll for completion or cancellation.
                        while (!done.Wait(500))
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                wc.CancelAsync();
                                done.Wait(3000);
                                UpdateLogger.Log("UpdateChecker: Download cancelled by user.");
                                SafeDeleteFile(tempZip);
                                return null;
                            }
                        }

                        if (downloadError != null)
                            throw downloadError;
                    }
                }

                UpdateLogger.Log("UpdateChecker: Download complete. Verifying SHA-256...");

                // Verify integrity before accepting the download.
                string actualHash = ComputeSha256(tempZip);
                if (!string.Equals(actualHash, info.Sha256,
                                   StringComparison.OrdinalIgnoreCase))
                {
                    UpdateLogger.Log(string.Format(
                        "UpdateChecker: SHA-256 mismatch! Expected={0}, Got={1}",
                        info.Sha256, actualHash));
                    SafeDeleteFile(tempZip);
                    return null;
                }

                UpdateLogger.Log("UpdateChecker: SHA-256 verified. Hash = " + actualHash);

                // Rename the verified download to its final name atomically.
                SafeDeleteFile(finalZip);
                File.Move(tempZip, finalZip);

                return finalZip;
            }
            catch (Exception ex)
            {
                UpdateLogger.LogException("UpdateChecker: Download failed", ex);
                SafeDeleteFile(tempZip);
                return null;
            }
        }

        /// <summary>
        /// Launches Updater.exe with the arguments needed to apply the update,
        /// then requests the calling application to close.
        /// </summary>
        /// <param name="zipPath">Full path to the verified update zip.</param>
        /// <param name="installDir">Directory where LanOra is installed.</param>
        /// <param name="mainExe">Full path to the main LanOra.exe.</param>
        public static void LaunchUpdater(string zipPath, string installDir, string mainExe)
        {
            string updaterPath = Path.Combine(installDir, "Updater.exe");

            if (!File.Exists(updaterPath))
            {
                UpdateLogger.Log("UpdateChecker: Updater.exe not found at " + updaterPath);
                throw new FileNotFoundException("Updater.exe not found.", updaterPath);
            }

            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            // Arguments passed to Updater.exe:
            //   <lanoraProcessId> <zipPath> <installDir> <mainExePath>
            string args = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"",
                pid, zipPath, installDir, mainExe);

            UpdateLogger.Log(string.Format(
                "UpdateChecker: Launching updater: {0} {1}", updaterPath, args));

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = updaterPath,
                Arguments       = args,
                UseShellExecute = true
            });
        }

        // ------------------------------------------------------------------
        // JSON parsing (manual – no external JSON library required)
        // ------------------------------------------------------------------

        /// <summary>
        /// Parses the minimal version.json format without any external library.
        /// Expected format:
        /// <code>
        /// {
        ///   "version":      "1.2.0",
        ///   "downloadUrl":  "https://...",
        ///   "sha256":       "HEXHASH",
        ///   "mandatory":    false,
        ///   "channel":      "stable",
        ///   "releaseNotes": "..."
        /// }
        /// </code>
        /// </summary>
        internal static VersionInfo ParseVersionJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new FormatException("Empty version manifest.");

            var info = new VersionInfo();

            info.Version      = ParseJsonVersion(json,  "version");
            info.DownloadUrl  = ParseJsonString(json,   "downloadUrl");
            info.Sha256       = ParseJsonString(json,   "sha256");
            info.Mandatory    = ParseJsonBool(json,     "mandatory");
            info.Channel      = ParseJsonString(json,   "channel") ?? "stable";
            info.ReleaseNotes = ParseJsonString(json,   "releaseNotes") ?? string.Empty;

            if (info.Version == null)
                throw new FormatException("version field missing or invalid in manifest.");

            return info;
        }

        private static string ParseJsonString(string json, string key)
        {
            // Matches: "key" : "value"  (handles optional spaces, escaped chars)
            Match m = Regex.Match(json,
                @"""" + Regex.Escape(key) + @"""\s*:\s*""((?:[^""\\]|\\.)*)""",
                RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : null;
        }

        private static Version ParseJsonVersion(string json, string key)
        {
            string s = ParseJsonString(json, key);
            if (string.IsNullOrEmpty(s)) return null;

            Version v;
            return Version.TryParse(s, out v) ? v : null;
        }

        private static bool ParseJsonBool(string json, string key)
        {
            Match m = Regex.Match(json,
                @"""" + Regex.Escape(key) + @"""\s*:\s*(true|false)",
                RegexOptions.IgnoreCase);
            return m.Success &&
                   string.Equals(m.Groups[1].Value, "true",
                                 StringComparison.OrdinalIgnoreCase);
        }

        // ------------------------------------------------------------------
        // SHA-256 helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Computes the SHA-256 hash of a file and returns it as a lowercase
        /// hex string.
        /// </summary>
        public static string ComputeSha256(string filePath)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open,
                                                  FileAccess.Read, FileShare.Read,
                                                  bufferSize: 65536,
                                                  useAsync: false))
            {
                byte[] hash = sha.ComputeHash(fs);
                var sb = new StringBuilder(64);
                foreach (byte b in hash)
                    sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }

        // ------------------------------------------------------------------
        // WebClient factory
        // ------------------------------------------------------------------

        private static WebClient CreateWebClient()
        {
            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.UserAgent] =
                string.Format("LanOra/{0} (Windows; .NET {1})",
                              LocalVersion,
                              Environment.Version);
            return wc;
        }

        // ------------------------------------------------------------------
        // File helpers
        // ------------------------------------------------------------------

        private static void SafeDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { /* ignore */ }
        }
    }
}
