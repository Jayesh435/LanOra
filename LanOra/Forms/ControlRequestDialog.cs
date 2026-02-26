using System;
using System.Drawing;
using System.Windows.Forms;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Modal permission dialog shown to the host when a viewer requests
    /// remote control.  The host can Allow or Deny the request.  If no
    /// action is taken within <see cref="TimeoutSeconds"/> the request is
    /// automatically denied.
    /// </summary>
    internal sealed class ControlRequestDialog : Form
    {
        // ------------------------------------------------------------------ //
        // Public result                                                       //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// True if the host clicked Allow; false if Denied or timed out.
        /// Read this after <c>ShowDialog()</c> returns.
        /// </summary>
        public bool Approved { get; private set; }

        // ------------------------------------------------------------------ //
        // Constants                                                           //
        // ------------------------------------------------------------------ //

        private const int TimeoutSeconds = 15;

        // ------------------------------------------------------------------ //
        // Private controls and state                                          //
        // ------------------------------------------------------------------ //

        private readonly Timer _countdownTimer  = new Timer { Interval = 1000 };
        private int            _remaining        = TimeoutSeconds;
        private Label          _lblCountdown;

        // ------------------------------------------------------------------ //
        // Constructor                                                         //
        // ------------------------------------------------------------------ //

        public ControlRequestDialog(string viewerHostName, string viewerIp)
        {
            BuildUi(viewerHostName, viewerIp);

            _countdownTimer.Tick += CountdownTick;
            _countdownTimer.Start();
        }

        // ------------------------------------------------------------------ //
        // UI construction                                                     //
        // ------------------------------------------------------------------ //

        private void BuildUi(string viewerHostName, string viewerIp)
        {
            // Form
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = "Remote Control Request";
            BackColor       = AppTheme.Background;
            ClientSize      = new Size(420, 210);
            StartPosition   = FormStartPosition.CenterScreen;
            MaximizeBox     = false;
            MinimizeBox     = false;

            // Icon strip
            var pnlTop = new Panel
            {
                BackColor = AppTheme.ErrorRed,
                Dock      = DockStyle.Top,
                Height    = 6
            };

            // Main message
            var lblMessage = new Label
            {
                AutoSize  = false,
                Font      = new Font(AppTheme.FontFamily, 10F),
                ForeColor = AppTheme.TextPrimary,
                Location  = new Point(20, 22),
                Size      = new Size(380, 60),
                Text      = string.Format(
                    "{0} ({1}) is requesting remote control of this machine.",
                    viewerHostName, viewerIp)
            };

            // Warning sub-text
            var lblWarning = new Label
            {
                AutoSize  = false,
                Font      = new Font(AppTheme.FontFamily, 8.5F),
                ForeColor = AppTheme.WarningYellow,
                Location  = new Point(20, 86),
                Size      = new Size(380, 20),
                Text      = "Only allow if you recognise and trust this user."
            };

            // Countdown
            _lblCountdown = new Label
            {
                AutoSize  = false,
                Font      = new Font(AppTheme.FontFamily, 8.5F),
                ForeColor = AppTheme.TextSecondary,
                Location  = new Point(20, 112),
                Size      = new Size(380, 20),
                Text      = string.Format("Auto-deny in {0} seconds…", _remaining)
            };

            // Allow button
            var btnAllow = new Button
            {
                BackColor = AppTheme.SuccessGreen,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location  = new Point(20, 148),
                Size      = new Size(180, 40),
                Text      = "Allow",
                Cursor    = Cursors.Hand
            };
            btnAllow.FlatAppearance.BorderSize = 0;
            btnAllow.FlatAppearance.MouseOverBackColor = AppTheme.SuccessGreenDark;
            btnAllow.Click += BtnAllow_Click;

            // Deny button
            var btnDeny = new Button
            {
                BackColor = AppTheme.ErrorRed,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location  = new Point(220, 148),
                Size      = new Size(180, 40),
                Text      = "Deny",
                Cursor    = Cursors.Hand
            };
            btnDeny.FlatAppearance.BorderSize = 0;
            btnDeny.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRedDark;
            btnDeny.Click += BtnDeny_Click;

            Controls.AddRange(new Control[] { pnlTop, lblMessage, lblWarning, _lblCountdown, btnAllow, btnDeny });
        }

        // ------------------------------------------------------------------ //
        // Event handlers                                                      //
        // ------------------------------------------------------------------ //

        private void BtnAllow_Click(object sender, EventArgs e)
        {
            _countdownTimer.Stop();
            Approved     = true;
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void BtnDeny_Click(object sender, EventArgs e)
        {
            _countdownTimer.Stop();
            Approved     = false;
            DialogResult = DialogResult.No;
            Close();
        }

        private void CountdownTick(object sender, EventArgs e)
        {
            _remaining--;
            _lblCountdown.Text = string.Format("Auto-deny in {0} seconds…", _remaining);

            if (_remaining <= 0)
            {
                _countdownTimer.Stop();
                Approved     = false;
                DialogResult = DialogResult.No;
                Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _countdownTimer.Dispose();
            base.Dispose(disposing);
        }
    }
}
