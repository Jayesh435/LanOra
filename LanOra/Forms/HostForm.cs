using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LanOra.Networking;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Host mode window. Displays the machine IP, a randomly generated 6-digit
    /// PIN, and Start / Stop controls. When started, the app broadcasts its
    /// presence via UDP so viewers can discover it automatically.
    /// </summary>
    public partial class HostForm : Form
    {
        private readonly ScreenServer _server = new ScreenServer();
        private readonly HostBeacon   _beacon = new HostBeacon();
        private readonly string       _pin;

        // Title-bar drag
        private Point _dragOffset;

        // Count connected viewers
        private int _viewerCount;

        public HostForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            _pin = GeneratePin();
            WireEvents();
            lblIpValue.Text = GetLocalIpAddress();
            lblPort.Text    = "Port: " + ScreenServer.Port;
            lblPinValue.Text = FormatPin(_pin);
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
            _server.StatusChanged      += msg => SafeInvoke(() => UpdateStatusBar(msg));
            _server.ClientConnected    += ip  => SafeInvoke(() => OnClientConnected(ip));
            _server.ClientDisconnected +=       () => SafeInvoke(OnClientDisconnected);
            _server.ErrorOccurred      += msg => SafeInvoke(() => ShowError(msg));
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

                // Update status indicator
                lblStatusDot.ForeColor = AppTheme.SuccessGreen;
                lblIpValue2.Text = "Hosting\u2026";
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
            _server.Stop();
            _beacon.Stop();

            btnStop.Visible  = false;
            btnStart.Visible = true;

            lblStatusDot.ForeColor = AppTheme.ErrorRed;
            lblIpValue2.Text       = "Not Hosting";
            lblViewerCount.Visible = false;
            _viewerCount = 0;

            lblStatusBar.Text = "Host Mode: Idle  |  " + AppTheme.Developer;
        }

        private void HostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.Stop();
            _beacon.Stop();
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
