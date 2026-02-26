using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LanOra.Networking;
using LanOra.Security;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Host mode window. Displays the machine IP, a randomly generated 6-digit
    /// PIN, and Start / Stop controls. When started, the app broadcasts its
    /// presence via UDP so viewers can discover it automatically.
    ///
    /// Secure remote control:
    ///   – "Allow Remote Control" checkbox enables the feature.
    ///   – When a viewer requests control a modal dialog gives the host 15 s
    ///     to Allow or Deny.
    ///   – A red banner is shown at the top while control is active.
    ///   – A small indicator flashes on every received input event.
    ///   – Control is immediately revoked when the checkbox is unchecked.
    /// </summary>
    public partial class HostForm : Form
    {
        private readonly ScreenServer   _server  = new ScreenServer();
        private readonly HostBeacon     _beacon  = new HostBeacon();
        private readonly ControlManager _control = new ControlManager();
        private readonly SessionLogger  _logger  = new SessionLogger();
        private readonly string         _pin;

        // Timer used to clear the activity-flash indicator after 200 ms
        private readonly Timer _flashTimer = new Timer { Interval = 200 };

        // Title-bar drag
        private Point _dragOffset;

        // Count connected viewers
        private int _viewerCount;

        public HostForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            _pin = GeneratePin();

            // Wire ControlManager and SessionLogger into the server
            _server.ControlManager = _control;
            _server.Logger         = _logger;

            WireEvents();
            lblIpValue.Text  = GetLocalIpAddress();
            lblPort.Text     = "Port: " + ScreenServer.Port;
            lblPinValue.Text = FormatPin(_pin);

            _flashTimer.Tick += (s, e) =>
            {
                _flashTimer.Stop();
                lblActivityFlash.Text = string.Empty;
            };
        }

        // ------------------------------------------------------------------ //
        // PIN generation                                                      //
        // ------------------------------------------------------------------ //

        private static string GeneratePin()
        {
            byte[] buf = new byte[4];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(buf);
            uint raw = BitConverter.ToUInt32(buf, 0) % 900000u;
            return (100000u + raw).ToString();
        }

        /// <summary>Formats a 6-digit PIN as "XXX XXX".</summary>
        private static string FormatPin(string pin)
            => pin.Length == 6 ? pin.Substring(0, 3) + " " + pin.Substring(3) : pin;

        // ------------------------------------------------------------------ //
        // Event wiring                                                        //
        // ------------------------------------------------------------------ //

        private void WireEvents()
        {
            _server.StatusChanged         += msg  => SafeInvoke(() => UpdateStatusBar(msg));
            _server.ClientConnected       += ip   => SafeInvoke(() => OnClientConnected(ip));
            _server.ClientDisconnected    +=        () => SafeInvoke(OnClientDisconnected);
            _server.ErrorOccurred         += msg  => SafeInvoke(() => ShowError(msg));
            _server.ControlRequestReceived += info => SafeInvoke(() => OnControlRequestReceived(info));
            _server.ControlReleased        +=        () => SafeInvoke(OnControlReleased);
            _server.InputActivityFlash     +=        () => SafeInvoke(FlashActivityIndicator);
        }

        // ------------------------------------------------------------------ //
        // Title-bar drag                                                      //
        // ------------------------------------------------------------------ //

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

        // ------------------------------------------------------------------ //
        // Title-bar buttons                                                   //
        // ------------------------------------------------------------------ //

        private void btnClose_Click(object sender, EventArgs e)    => Close();
        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // ------------------------------------------------------------------ //
        // UI event handlers                                                   //
        // ------------------------------------------------------------------ //

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _server.Pin = _pin;
                _server.Start();
                _beacon.Start(Environment.MachineName, lblIpValue.Text, ScreenServer.Port);

                btnStart.Visible = false;
                btnStop.Visible  = true;

                lblStatusDot.ForeColor = AppTheme.SuccessGreen;
                lblIpValue2.Text       = "Hosting\u2026";
                lblViewerCount.Text    = "Connected Viewers: 0";
                lblViewerCount.Visible = true;
                _viewerCount = 0;

                lblStatusBar.Text = "Host Mode: Hosting  |  " + AppTheme.Developer;
            }
            catch (Exception ex)
            {
                ShowError("Failed to start: " + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            RevokeControlIfActive();
            _server.Stop();
            _beacon.Stop();

            btnStop.Visible  = false;
            btnStart.Visible = true;

            lblStatusDot.ForeColor   = AppTheme.ErrorRed;
            lblIpValue2.Text         = "Not Hosting";
            lblViewerCount.Visible   = false;
            pnlControlBanner.Visible = false;
            _viewerCount = 0;

            lblStatusBar.Text = "Host Mode: Idle  |  " + AppTheme.Developer;
        }

        private void HostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.Stop();
            _beacon.Stop();
            _flashTimer.Stop();
        }

        /// <summary>
        /// Fires when the "Allow Remote Control" checkbox changes state.
        /// Immediately revokes active control if unchecked.
        /// </summary>
        private void chkAllowControl_CheckedChanged(object sender, EventArgs e)
        {
            bool allow = chkAllowControl.Checked;
            _control.SetAllowEnabled(allow);

            if (!allow && pnlControlBanner.Visible)
            {
                // Revoke active control and notify viewer
                _logger.Log("Control Revoked – host unchecked Allow Remote Control");
                _server.SendControlRevoke();
                HideControlBanner();
            }
        }

        // ------------------------------------------------------------------ //
        // Server callback handlers (already on UI thread via SafeInvoke)     //
        // ------------------------------------------------------------------ //

        private void OnClientConnected(string ip)
        {
            _viewerCount++;
            lblStatusDot.ForeColor = AppTheme.SuccessGreen;
            lblViewerCount.Text    = string.Format("Connected Viewers: {0}", _viewerCount);
        }

        private void OnClientDisconnected()
        {
            if (_viewerCount > 0) _viewerCount--;
            lblViewerCount.Text = string.Format("Connected Viewers: {0}", _viewerCount);
            if (_viewerCount == 0)
                lblStatusDot.ForeColor = AppTheme.SuccessGreen; // still hosting

            // If control was active, the connection drop automatically cleared
            // ControlManager state; just hide the banner.
            HideControlBanner();
        }

        /// <summary>
        /// Viewer is requesting remote control.  Show the permission dialog.
        /// info format: "MachineName|IP"
        /// </summary>
        private void OnControlRequestReceived(string info)
        {
            string machineName = info;
            string ip          = string.Empty;

            int sep = info.IndexOf('|');
            if (sep >= 0)
            {
                machineName = info.Substring(0, sep);
                ip          = info.Substring(sep + 1);
            }

            using (var dlg = new ControlRequestDialog(machineName, ip))
            {
                dlg.ShowDialog(this);
                bool approved = dlg.Approved;

                _server.SendControlResponse(approved);

                if (approved)
                {
                    _control.ApproveControl();
                    _logger.Log("Control Approved");
                    ShowControlBanner();
                }
                else
                {
                    _logger.Log("Control Denied");
                }
            }
        }

        /// <summary>Viewer sent ControlRelease (viewer released control).</summary>
        private void OnControlReleased()
        {
            _control.RevokeControl();
            HideControlBanner();
        }

        /// <summary>Flashes the activity indicator for 200 ms.</summary>
        private void FlashActivityIndicator()
        {
            lblActivityFlash.Text = "\u25CF INPUT";
            _flashTimer.Stop();
            _flashTimer.Start();
        }

        // ------------------------------------------------------------------ //
        // Control banner helpers                                              //
        // ------------------------------------------------------------------ //

        private void ShowControlBanner()
        {
            pnlControlBanner.Visible = true;
        }

        private void HideControlBanner()
        {
            pnlControlBanner.Visible = false;
            lblActivityFlash.Text    = string.Empty;
            _flashTimer.Stop();
        }

        private void RevokeControlIfActive()
        {
            if (_control.IsControlActive || pnlControlBanner.Visible)
            {
                _control.RevokeControl();
                _server.SendControlRevoke();
                _logger.Log("Control Revoked");
                HideControlBanner();
            }
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        private void UpdateStatusBar(string message)
        {
            // Status bar text is updated directly by btnStart/Stop_Click handlers;
            // detailed server messages are informational only.
        }

        private void ShowError(string message) =>
            MessageBox.Show(message, "Host Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static string GetLocalIpAddress()
        {
            try
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork,
                                                SocketType.Dgram, ProtocolType.Udp))
                {
                    sock.Connect("8.8.8.8", 65530);
                    return ((IPEndPoint)sock.LocalEndPoint).Address.ToString();
                }
            }
            catch
            {
                foreach (IPAddress addr in Dns.GetHostAddresses(Dns.GetHostName()))
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                        return addr.ToString();
                return "127.0.0.1";
            }
        }

        /// <summary>Thread-safe UI update helper.</summary>
        private void SafeInvoke(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }
    }
}
