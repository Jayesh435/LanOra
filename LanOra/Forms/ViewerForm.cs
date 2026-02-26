using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LanOra.Networking;

namespace LanOra.Forms
{
    /// <summary>
    /// Viewer mode window. Automatically discovers hosts via UDP broadcast,
    /// shows them in a list, and connects using the PIN entered by the user.
    /// </summary>
    public partial class ViewerForm : Form
    {
        private readonly ScreenClient _client  = new ScreenClient();
        private readonly HostScanner  _scanner = new HostScanner();
        private Bitmap _currentFrame;

        public ViewerForm()
        {
            InitializeComponent();
            WireEvents();
            _scanner.Start();
        }

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

        // ------------------------------------------------------------------ //
        // Host list                                                           //
        // ------------------------------------------------------------------ //

        private void RefreshHostList()
        {
            IReadOnlyList<HostInfo> hosts = _scanner.Hosts;

            // Remember current selection (by display text)
            string selected = lstHosts.SelectedItem?.ToString();

            lstHosts.Items.Clear();
            foreach (HostInfo h in hosts)
                lstHosts.Items.Add(h);

            // Restore selection if still present
            if (selected != null)
                for (int i = 0; i < lstHosts.Items.Count; i++)
                    if (lstHosts.Items[i].ToString() == selected)
                        lstHosts.SelectedIndex = i;
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshHostList();

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

            string pin = txtPin.Text.Trim();
            if (string.IsNullOrEmpty(pin))
            {
                MessageBox.Show("Please enter the PIN shown on the host machine.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled    = false;
            btnDisconnect.Enabled = true;
            lstHosts.Enabled      = false;
            txtPin.Enabled        = false;
            btnRefresh.Enabled    = false;

            _client.Pin = pin;
            _client.Connect(host.IpAddress);
        }

        private void btnDisconnect_Click(object sender, EventArgs e) => _client.Disconnect();

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            btnConnect.Enabled    = true;
            btnDisconnect.Enabled = false;
            lstHosts.Enabled      = true;
            txtPin.Enabled        = true;
            btnRefresh.Enabled    = true;
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
            lblStatus.Text = "Status: " + message;
            lblStatusDot.ForeColor = message.StartsWith("Connected", StringComparison.OrdinalIgnoreCase)
                                     ? Color.LimeGreen
                                     : Color.Orange;
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
