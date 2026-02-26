using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LanOra.Networking;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Viewer mode window. Automatically discovers hosts via UDP broadcast,
    /// shows them in a list, and connects using the 6-digit PIN entered by the
    /// user. Displays a live performance overlay (FPS / KB/s) while streaming.
    /// </summary>
    public partial class ViewerForm : Form
    {
        private readonly ScreenClient _client  = new ScreenClient();
        private readonly HostScanner  _scanner = new HostScanner();
        private Bitmap _currentFrame;

        // Performance overlay timer (updates the overlay label every second)
        private readonly Timer _perfTimer = new Timer { Interval = 1000 };

        // Title-bar drag support
        private Point _dragOffset;

        public ViewerForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            WireEvents();
            WirePinBoxes();
            _scanner.Start();
            _perfTimer.Tick += (s, e) => UpdatePerfOverlay();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            _perfTimer.Stop();
            _client.Disconnect();
            _scanner.Stop();
            Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        // ------------------------------------------------------------------ //
        // Event wiring                                                        //
        // ------------------------------------------------------------------ //

        private void WireEvents()
        {
            _client.FrameReceived  += bmp => SafeInvoke(() => DisplayFrame(bmp));
            _client.StatusChanged  += msg => SafeInvoke(() => UpdateStatus(msg));
            _client.Disconnected   +=       () => SafeInvoke(OnDisconnected);
            _client.ErrorOccurred  += msg => SafeInvoke(() => ShowError(msg));

            _scanner.HostsUpdated  += () => SafeInvoke(RefreshHostList);
        }

        /// <summary>
        /// Wires keyboard handling on each PIN box so digits auto-advance the
        /// cursor and the backspace key moves focus backwards.
        /// </summary>
        private void WirePinBoxes()
        {
            TextBox[] boxes = PinBoxes;
            for (int i = 0; i < boxes.Length; i++)
            {
                int idx = i; // capture for lambda
                boxes[i].KeyPress += (s, e) =>
                {
                    // Allow only digits
                    if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                        return;
                    }
                    // Advance focus after a digit is entered
                    if (char.IsDigit(e.KeyChar) && idx < boxes.Length - 1)
                        BeginInvoke((Action)(() => boxes[idx + 1].Focus()));
                };
                boxes[i].KeyDown += (s, e) =>
                {
                    // Move focus backward on backspace if box is empty
                    if (e.KeyCode == Keys.Back &&
                        string.IsNullOrEmpty(boxes[idx].Text) &&
                        idx > 0)
                    {
                        boxes[idx - 1].Focus();
                        boxes[idx - 1].Clear();
                        e.SuppressKeyPress = true;
                    }
                };
            }
        }

        // ------------------------------------------------------------------ //
        // PIN helper                                                          //
        // ------------------------------------------------------------------ //

        private TextBox[] PinBoxes =>
            new[] { txtPin1, txtPin2, txtPin3, txtPin4, txtPin5, txtPin6 };

        /// <summary>Returns the concatenated digits from all 6 PIN boxes.</summary>
        private string GetPin()
        {
            var sb = new System.Text.StringBuilder(6);
            foreach (TextBox tb in PinBoxes)
                sb.Append(tb.Text);
            return sb.ToString();
        }

        /// <summary>Clears all 6 PIN boxes.</summary>
        private void ClearPin()
        {
            foreach (TextBox tb in PinBoxes)
                tb.Clear();
        }

        /// <summary>Enables or disables all 6 PIN boxes.</summary>
        private void SetPinBoxesEnabled(bool enabled)
        {
            foreach (TextBox tb in PinBoxes)
                tb.Enabled = enabled;
        }

        // ------------------------------------------------------------------ //
        // Host list                                                           //
        // ------------------------------------------------------------------ //

        private void RefreshHostList()
        {
            IReadOnlyList<HostInfo> hosts = _scanner.Hosts;

            string selected = lstHosts.SelectedItem?.ToString();

            lstHosts.Items.Clear();
            foreach (HostInfo h in hosts)
                lstHosts.Items.Add(h);

            if (selected != null)
                for (int i = 0; i < lstHosts.Items.Count; i++)
                    if (lstHosts.Items[i].ToString() == selected)
                        lstHosts.SelectedIndex = i;
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshHostList();

        private void lstHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstHosts.SelectedItem is HostInfo host)
                lblPinCaption.Text = string.Format("Enter PIN for {0}:", host.HostName);
            else
                lblPinCaption.Text = "Enter PIN:";
        }

        // ------------------------------------------------------------------ //
        // Connect / Disconnect                                                //
        // ------------------------------------------------------------------ //

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!(lstHosts.SelectedItem is HostInfo host))
            {
                MessageBox.Show("Please select a host from the list.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pin = GetPin();
            if (pin.Length < 6)
            {
                MessageBox.Show("Please enter the full 6-digit PIN shown on the host machine.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled    = false;
            btnDisconnect.Enabled = true;
            lstHosts.Enabled      = false;
            SetPinBoxesEnabled(false);
            btnRefresh.Enabled    = false;

            lblStatusDot.ForeColor = AppTheme.WarningYellow;
            lblStatusBar.Text      = "Viewer Mode: Connecting  |  " + AppTheme.Developer;

            _client.Pin = pin;
            _client.Connect(host.IpAddress);
            _perfTimer.Start();
        }

        private void btnDisconnect_Click(object sender, EventArgs e) => _client.Disconnect();

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _perfTimer.Stop();
            _client.Disconnect();
            _scanner.Stop();

            Bitmap old = _currentFrame;
            _currentFrame = null;
            old?.Dispose();
        }

        // ------------------------------------------------------------------ //
        // Client callback handlers (already on UI thread via SafeInvoke)     //
        // ------------------------------------------------------------------ //

        private void DisplayFrame(Bitmap newFrame)
        {
            Bitmap old    = _currentFrame;
            _currentFrame = newFrame;
            picScreen.Image = _currentFrame;
            picScreen.Refresh();
            old?.Dispose();
        }

        private void OnDisconnected()
        {
            _perfTimer.Stop();
            btnConnect.Enabled    = true;
            btnDisconnect.Enabled = false;
            lstHosts.Enabled      = true;
            SetPinBoxesEnabled(true);
            btnRefresh.Enabled    = true;

            lblStatusDot.ForeColor = AppTheme.ErrorRed;
            lblStatusBar.Text      = "Viewer Mode: Discovery  |  " + AppTheme.Developer;

            picScreen.Image        = null;
            lblPerfOverlay.Text    = string.Empty;
            lblPerfOverlay.Visible = false;

            ClearPin();

            Bitmap old = _currentFrame;
            _currentFrame = null;
            old?.Dispose();
        }

        // ------------------------------------------------------------------ //
        // Performance overlay                                                 //
        // ------------------------------------------------------------------ //

        private void UpdatePerfOverlay()
        {
            double fps = _client.Performance.Fps;
            double kbs = _client.Performance.KbPerSecond;

            lblPerfOverlay.Text    = string.Format("FPS: {0:F1}  |  {1:F0} KB/s", fps, kbs);
            lblPerfOverlay.Visible = true;
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        private void UpdateStatus(string message)
        {
            if (message.StartsWith("Connected", StringComparison.OrdinalIgnoreCase))
            {
                lblStatusDot.ForeColor = AppTheme.SuccessGreen;
                lblStatusBar.Text      = "Viewer Mode: Connected  |  " + AppTheme.Developer;
            }
            else if (message.StartsWith("Connecting", StringComparison.OrdinalIgnoreCase))
            {
                lblStatusDot.ForeColor = AppTheme.WarningYellow;
                lblStatusBar.Text      = "Viewer Mode: Connecting  |  " + AppTheme.Developer;
            }
            // Other messages (disconnected, errors) handled by OnDisconnected / ShowError
        }

        private void ShowError(string message) =>
            MessageBox.Show(message, "Viewer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>Thread-safe UI update helper.</summary>
        private void SafeInvoke(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }
    }
}

