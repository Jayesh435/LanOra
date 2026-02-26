using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using LanOra.Input;
using LanOra.Networking;
using LanOra.Theme;

namespace LanOra.Forms
{
    /// <summary>
    /// Viewer mode window. Automatically discovers hosts via UDP broadcast,
    /// shows them in a list, and connects using the 6-digit PIN entered by the
    /// user. Displays a live performance overlay (FPS / KB/s) while streaming.
    ///
    /// Secure remote control:
    ///   – "Request Control" button sends a control request to the host.
    ///   – While the request is pending a yellow badge is displayed.
    ///   – If approved the badge turns green and input is captured from picScreen.
    ///   – "Release Control" button reverts to view-only mode.
    ///   – Control is revoked automatically on disconnect or host-side revoke.
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

        // Track control state locally (for UI decisions)
        private bool _controlActive;

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
            _client.FrameReceived   += bmp => SafeInvoke(() => DisplayFrame(bmp));
            _client.StatusChanged   += msg => SafeInvoke(() => UpdateStatus(msg));
            _client.Disconnected    +=       () => SafeInvoke(OnDisconnected);
            _client.ErrorOccurred   += msg => SafeInvoke(() => ShowError(msg));
            _client.ControlApproved +=       () => SafeInvoke(OnControlApproved);
            _client.ControlDenied   +=       () => SafeInvoke(OnControlDenied);
            _client.ControlRevoked  +=       () => SafeInvoke(OnControlRevoked);

            _scanner.HostsUpdated   += () => SafeInvoke(RefreshHostList);
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

            btnConnect.Enabled         = false;
            btnDisconnect.Enabled      = true;
            btnRequestControl.Enabled  = false;
            lstHosts.Enabled           = false;
            SetPinBoxesEnabled(false);
            btnRefresh.Enabled         = false;

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
        // Remote control – request / release                                 //
        // ------------------------------------------------------------------ //

        private void btnRequestControl_Click(object sender, EventArgs e)
        {
            if (_controlActive)
            {
                // Release control
                _client.SendControlRelease();
                SetControlUi(false, false);
            }
            else
            {
                // Request control
                string viewerInfo = string.Format("{0}|{1}",
                    Environment.MachineName,
                    GetLocalIpAddress());
                _client.SendControlRequest(viewerInfo);

                // Show pending badge
                ShowControlBadge("Control Request Pending\u2026", AppTheme.WarningYellow);
                btnRequestControl.Enabled = false;
            }
        }

        private void OnControlApproved()
        {
            _controlActive = true;
            SetControlUi(true, true);

            // Attach input event handlers to picScreen
            picScreen.MouseMove  += PicScreen_MouseMove;
            picScreen.MouseDown  += PicScreen_MouseDown;
            picScreen.MouseUp    += PicScreen_MouseUp;
            picScreen.MouseWheel += PicScreen_MouseWheel;
            picScreen.KeyDown    += PicScreen_KeyDown;
            picScreen.KeyUp      += PicScreen_KeyUp;
            picScreen.Focus();
        }

        private void OnControlDenied()
        {
            _controlActive = false;
            ShowControlBadge(string.Empty, Color.Transparent);
            lblControlBadge.Visible  = false;
            btnRequestControl.Text    = "Request Control";
            btnRequestControl.Enabled = true;
        }

        private void OnControlRevoked()
        {
            SetControlUi(false, false);
        }

        private void SetControlUi(bool controlActive, bool showBadge)
        {
            _controlActive = controlActive;

            if (controlActive)
            {
                btnRequestControl.Text      = "Release Control";
                btnRequestControl.BackColor = AppTheme.ErrorRed;
                btnRequestControl.FlatAppearance.MouseOverBackColor = AppTheme.ErrorRedDark;
                btnRequestControl.Enabled   = true;
                ShowControlBadge("\u25CF  CONTROL ACTIVE", AppTheme.SuccessGreen);
            }
            else
            {
                // Detach input handlers
                picScreen.MouseMove  -= PicScreen_MouseMove;
                picScreen.MouseDown  -= PicScreen_MouseDown;
                picScreen.MouseUp    -= PicScreen_MouseUp;
                picScreen.MouseWheel -= PicScreen_MouseWheel;
                picScreen.KeyDown    -= PicScreen_KeyDown;
                picScreen.KeyUp      -= PicScreen_KeyUp;

                btnRequestControl.Text      = "Request Control";
                btnRequestControl.BackColor = AppTheme.SuccessGreen;
                btnRequestControl.FlatAppearance.MouseOverBackColor = AppTheme.SuccessGreenDark;
                btnRequestControl.Enabled   = true;
                lblControlBadge.Visible     = false;
            }
        }

        private void ShowControlBadge(string text, Color foreColor)
        {
            lblControlBadge.Text      = text;
            lblControlBadge.ForeColor = foreColor;
            lblControlBadge.Visible   = !string.IsNullOrEmpty(text);
        }

        // ------------------------------------------------------------------ //
        // Input capture – mouse                                               //
        // ------------------------------------------------------------------ //

        private void PicScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_controlActive) return;
            Rectangle img = GetImageBounds();
            if (!img.Contains(e.Location)) return;

            _client.SendInputPacket(new InputPacket
            {
                EventType    = InputEventType.MouseMove,
                X            = e.X - img.X,
                Y            = e.Y - img.Y,
                ViewerWidth  = img.Width,
                ViewerHeight = img.Height
            });
        }

        private void PicScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_controlActive) return;
            Rectangle img = GetImageBounds();

            _client.SendInputPacket(new InputPacket
            {
                EventType    = InputEventType.MouseDown,
                X            = e.X - img.X,
                Y            = e.Y - img.Y,
                Button       = (int)e.Button,
                ViewerWidth  = img.Width,
                ViewerHeight = img.Height
            });
        }

        private void PicScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_controlActive) return;
            Rectangle img = GetImageBounds();

            _client.SendInputPacket(new InputPacket
            {
                EventType    = InputEventType.MouseUp,
                X            = e.X - img.X,
                Y            = e.Y - img.Y,
                Button       = (int)e.Button,
                ViewerWidth  = img.Width,
                ViewerHeight = img.Height
            });
        }

        private void PicScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_controlActive) return;

            _client.SendInputPacket(new InputPacket
            {
                EventType = InputEventType.MouseWheel,
                Delta     = e.Delta
            });
        }

        // ------------------------------------------------------------------ //
        // Input capture – keyboard                                            //
        // ------------------------------------------------------------------ //

        private void PicScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_controlActive) return;
            e.Handled = true;
            e.SuppressKeyPress = true;

            _client.SendInputPacket(new InputPacket
            {
                EventType = InputEventType.KeyDown,
                KeyCode   = (int)e.KeyCode
            });
        }

        private void PicScreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_controlActive) return;
            e.Handled = true;

            _client.SendInputPacket(new InputPacket
            {
                EventType = InputEventType.KeyUp,
                KeyCode   = (int)e.KeyCode
            });
        }

        // ------------------------------------------------------------------ //
        // Image bounds helper (for coordinate scaling with SizeMode.Zoom)    //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Returns the actual pixel rectangle of the displayed image within
        /// <see cref="picScreen"/> when <see cref="PictureBoxSizeMode.Zoom"/>
        /// is active (accounts for letter-boxing / pillar-boxing).
        /// </summary>
        private Rectangle GetImageBounds()
        {
            if (picScreen.Image == null)
                return picScreen.ClientRectangle;

            float imgAspect = (float)picScreen.Image.Width  / picScreen.Image.Height;
            float picAspect = (float)picScreen.Width        / picScreen.Height;

            int x, y, w, h;
            if (imgAspect > picAspect)
            {
                w = picScreen.Width;
                h = (int)(picScreen.Width / imgAspect);
                x = 0;
                y = (picScreen.Height - h) / 2;
            }
            else
            {
                h = picScreen.Height;
                w = (int)(picScreen.Height * imgAspect);
                x = (picScreen.Width - w) / 2;
                y = 0;
            }
            return new Rectangle(x, y, Math.Max(1, w), Math.Max(1, h));
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
            _controlActive = false;

            // Detach any input handlers that may be active
            picScreen.MouseMove  -= PicScreen_MouseMove;
            picScreen.MouseDown  -= PicScreen_MouseDown;
            picScreen.MouseUp    -= PicScreen_MouseUp;
            picScreen.MouseWheel -= PicScreen_MouseWheel;
            picScreen.KeyDown    -= PicScreen_KeyDown;
            picScreen.KeyUp      -= PicScreen_KeyUp;

            btnConnect.Enabled         = true;
            btnDisconnect.Enabled      = false;
            btnRequestControl.Enabled  = false;
            btnRequestControl.Text     = "Request Control";
            btnRequestControl.BackColor = AppTheme.SuccessGreen;
            lstHosts.Enabled           = true;
            SetPinBoxesEnabled(true);
            btnRefresh.Enabled         = true;

            lblStatusDot.ForeColor   = AppTheme.ErrorRed;
            lblStatusBar.Text        = "Viewer Mode: Discovery  |  " + AppTheme.Developer;

            picScreen.Image          = null;
            lblPerfOverlay.Text      = string.Empty;
            lblPerfOverlay.Visible   = false;
            lblControlBadge.Visible  = false;

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
                lblStatusDot.ForeColor    = AppTheme.SuccessGreen;
                lblStatusBar.Text         = "Viewer Mode: Connected  |  " + AppTheme.Developer;
                btnRequestControl.Enabled = true;
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

        private static string GetLocalIpAddress()
        {
            try
            {
                using (var sock = new System.Net.Sockets.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork,
                    System.Net.Sockets.SocketType.Dgram,
                    System.Net.Sockets.ProtocolType.Udp))
                {
                    sock.Connect("8.8.8.8", 65530);
                    return ((System.Net.IPEndPoint)sock.LocalEndPoint).Address.ToString();
                }
            }
            catch
            {
                foreach (System.Net.IPAddress addr in
                         System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
