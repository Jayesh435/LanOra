using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using LanOra.Theme;
using LanOra.Utilities;

namespace LanOra.Forms
{
    /// <summary>
    /// Modal dialog shown when a newer version of LanOra is available.
    /// Displays release information, a download progress bar, and buttons
    /// to update immediately, postpone, or (in mandatory mode) accept only.
    /// </summary>
    internal partial class UpdateDialog : Form
    {
        // ------------------------------------------------------------------
        // Fields
        // ------------------------------------------------------------------

        private readonly VersionInfo         _versionInfo;
        private          CancellationTokenSource _cts;
        private          string              _downloadedZip;

        // Title-bar drag support
        private Point _dragOffset;

        // ------------------------------------------------------------------
        // Constructor
        // ------------------------------------------------------------------

        /// <param name="info">Remote version manifest returned by UpdateChecker.</param>
        public UpdateDialog(VersionInfo info)
        {
            if (info == null) throw new ArgumentNullException("info");
            _versionInfo = info;

            InitializeComponent();
            DoubleBuffered = true;

            PopulateUI();

            if (info.Mandatory)
                DisableSkip();
        }

        // ------------------------------------------------------------------
        // UI population
        // ------------------------------------------------------------------

        private void PopulateUI()
        {
            lblCurrentVersion.Text = "Current version:  " + UpdateChecker.LocalVersion;
            lblNewVersion.Text     = "New version:       " + _versionInfo.Version;

            lblMandatory.Visible = _versionInfo.Mandatory;

            if (!string.IsNullOrEmpty(_versionInfo.ReleaseNotes))
                txtReleaseNotes.Text = _versionInfo.ReleaseNotes;
            else
                txtReleaseNotes.Text = "(No release notes provided.)";
        }

        private void DisableSkip()
        {
            btnSkip.Enabled = false;
            btnSkip.Visible = false;
            lblMandatory.Text = "⚠  This update is mandatory and cannot be skipped.";
        }

        // ------------------------------------------------------------------
        // Title-bar drag
        // ------------------------------------------------------------------

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _dragOffset = e.Location;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Location = new Point(
                    Location.X + e.X - _dragOffset.X,
                    Location.Y + e.Y - _dragOffset.Y);
        }

        // ------------------------------------------------------------------
        // Button handlers
        // ------------------------------------------------------------------

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            StartDownload();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            UpdateLogger.Log("UpdateDialog: User skipped update to " + _versionInfo.Version);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CancelDownload();
            if (!_versionInfo.Mandatory)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Prevent closing during download in mandatory-update mode.
            if (_versionInfo.Mandatory &&
                btnUpdate.Enabled == false &&
                _downloadedZip == null)
            {
                e.Cancel = true;
                return;
            }

            CancelDownload();
            base.OnFormClosing(e);
        }

        // ------------------------------------------------------------------
        // Download orchestration
        // ------------------------------------------------------------------

        private void StartDownload()
        {
            btnUpdate.Enabled = false;
            btnSkip.Enabled   = false;
            progressBar.Visible  = true;
            lblStatus.Text       = "Connecting…";
            lblStatus.Visible    = true;

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            DownloadProgressCallback onProgress = (percent, received, total) =>
            {
                SafeInvoke(() =>
                {
                    progressBar.Value = Math.Max(0, Math.Min(100, percent));
                    lblStatus.Text = string.Format(
                        "Downloading… {0}% ({1:F1} MB / {2:F1} MB)",
                        percent,
                        received / (1024.0 * 1024.0),
                        total > 0 ? total / (1024.0 * 1024.0) : 0.0);
                });
            };

            UpdateLogger.Log("UpdateDialog: Download started for version " + _versionInfo.Version);

            // Run the blocking download on a background thread.
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string zip = UpdateChecker.DownloadUpdate(_versionInfo, onProgress, token);

                SafeInvoke(() => OnDownloadComplete(zip));
            });
        }

        private void OnDownloadComplete(string zipPath)
        {
            if (zipPath == null)
            {
                // Download failed or was cancelled.
                progressBar.Value   = 0;
                lblStatus.Text      = "Download failed or was cancelled.";
                btnUpdate.Enabled   = true;
                btnSkip.Enabled     = !_versionInfo.Mandatory;
                UpdateLogger.Log("UpdateDialog: Download did not complete successfully.");
                return;
            }

            _downloadedZip = zipPath;
            lblStatus.Text = "Download complete. Launching updater…";
            progressBar.Value = 100;

            UpdateLogger.Log("UpdateDialog: Download complete. Zip = " + zipPath);

            // Brief pause so the user can see the 100 % message.
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(800);
                SafeInvoke(ApplyUpdate);
            });
        }

        private void ApplyUpdate()
        {
            try
            {
                string installDir = AppDomain.CurrentDomain.BaseDirectory;
                string mainExe    = System.Reflection.Assembly.GetExecutingAssembly().Location;

                UpdateChecker.LaunchUpdater(_downloadedZip, installDir, mainExe);
                UpdateLogger.Log("UpdateDialog: Updater launched. Closing LanOra.");

                // Signal the main application to exit.
                DialogResult = DialogResult.OK;
                Application.Exit();
            }
            catch (Exception ex)
            {
                UpdateLogger.LogException("UpdateDialog: Failed to launch updater", ex);
                MessageBox.Show(
                    "Failed to launch the updater:\n" + ex.Message,
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text    = "Update failed. Please try again.";
                btnUpdate.Enabled = true;
                btnSkip.Enabled   = !_versionInfo.Mandatory;
            }
        }

        private void CancelDownload()
        {
            try { _cts?.Cancel(); } catch { /* ignore */ }
        }

        // ------------------------------------------------------------------
        // Thread-safe UI helper
        // ------------------------------------------------------------------

        private void SafeInvoke(Action action)
        {
            if (InvokeRequired) BeginInvoke(action);
            else action();
        }
    }
}
