using System;
using System.Drawing;
using System.Windows.Forms;
using LANMonitor.Viewer.Networking;

namespace LANMonitor.Viewer.Forms
{
    /// <summary>
    /// Main window for the LAN Screen Viewer application.
    /// Connects to a ScreenServer and displays the live screen stream.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly ScreenClient _client = new ScreenClient();
        private Bitmap _currentFrame;

        public MainForm()
        {
            InitializeComponent();
            WireClientEvents();
        }

        // ------------------------------------------------------------------ //
        // Event wiring                                                        //
        // ------------------------------------------------------------------ //

        private void WireClientEvents()
        {
            _client.FrameReceived  += bmp  => SafeInvoke(() => DisplayFrame(bmp));
            _client.StatusChanged  += msg  => SafeInvoke(() => UpdateStatus(msg));
            _client.Disconnected   +=        () => SafeInvoke(() => OnDisconnected());
            _client.ErrorOccurred  += msg  => SafeInvoke(() => ShowError(msg));
        }

        // ------------------------------------------------------------------ //
        // UI event handlers                                                   //
        // ------------------------------------------------------------------ //

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIpAddress.Text.Trim();

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Please enter the server IP address.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled    = false;
            btnDisconnect.Enabled = true;
            txtIpAddress.Enabled  = false;

            _client.Connect(ip);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client.Disconnect();

            // Dispose the current frame if any
            Bitmap old = _currentFrame;
            _currentFrame = null;
            old?.Dispose();
        }

        // ------------------------------------------------------------------ //
        // Client callback handlers (already on UI thread via SafeInvoke)     //
        // ------------------------------------------------------------------ //

        private void DisplayFrame(Bitmap newFrame)
        {
            // Swap frame references and update picture box
            Bitmap old     = _currentFrame;
            _currentFrame  = newFrame;

            picScreen.Image = _currentFrame;
            picScreen.Refresh();

            // Dispose the old frame after assigning the new one
            old?.Dispose();
        }

        private void OnDisconnected()
        {
            btnConnect.Enabled    = true;
            btnDisconnect.Enabled = false;
            txtIpAddress.Enabled  = true;
            lblStatusDot.ForeColor = Color.Red;
            picScreen.Image        = null;

            Bitmap old = _currentFrame;
            _currentFrame = null;
            old?.Dispose();
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        private void UpdateStatus(string message)
        {
            lblStatus.Text         = "Status: " + message;
            lblStatusDot.ForeColor = message.StartsWith("Connected", StringComparison.OrdinalIgnoreCase)
                                     ? Color.LimeGreen
                                     : Color.Orange;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Viewer Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Thread-safe UI update helper.</summary>
        private void SafeInvoke(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }
    }
}
