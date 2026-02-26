using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LANMonitor.Server.Networking;

namespace LANMonitor.Server.Forms
{
    /// <summary>
    /// Main window for the LAN Screen Server application.
    /// Shows the local IP, port, and connection status.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly ScreenServer _server = new ScreenServer();

        public MainForm()
        {
            InitializeComponent();
            WireServerEvents();
            DisplayLocalIp();
            lblPort.Text = "Port: " + ScreenServer.Port;
        }

        // ------------------------------------------------------------------ //
        // Event wiring                                                        //
        // ------------------------------------------------------------------ //

        private void WireServerEvents()
        {
            _server.StatusChanged      += msg => SafeInvoke(() => UpdateStatus(msg));
            _server.ClientConnected    += ip  => SafeInvoke(() => OnClientConnected(ip));
            _server.ClientDisconnected +=       () => SafeInvoke(() => OnClientDisconnected());
            _server.ErrorOccurred      += msg => SafeInvoke(() => ShowError(msg));
        }

        // ------------------------------------------------------------------ //
        // UI event handlers                                                   //
        // ------------------------------------------------------------------ //

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _server.Start();
                btnStart.Enabled = false;
                btnStop.Enabled  = true;
            }
            catch (Exception ex)
            {
                ShowError("Failed to start server: " + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _server.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled  = false;
            UpdateStatus("Server stopped.");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.Stop();
        }

        // ------------------------------------------------------------------ //
        // Server callback handlers (already on UI thread via SafeInvoke)     //
        // ------------------------------------------------------------------ //

        private void OnClientConnected(string ip)
        {
            lblStatusDot.ForeColor = System.Drawing.Color.LimeGreen;
            UpdateStatus("Connected: " + ip);
        }

        private void OnClientDisconnected()
        {
            lblStatusDot.ForeColor = System.Drawing.Color.Red;
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        private void DisplayLocalIp()
        {
            lblIpValue.Text = GetLocalIpAddress();
        }

        private static string GetLocalIpAddress()
        {
            // UDP destination used only to identify the LAN-facing interface; no data is sent
            const string RemoteDetectionHost = "8.8.8.8";
            const int    RemoteDetectionPort = 65530;

            try
            {
                // UDP connect trick: no data is sent; the OS selects the correct local interface
                using (Socket sock = new Socket(AddressFamily.InterNetwork,
                                                SocketType.Dgram, ProtocolType.Udp))
                {
                    sock.Connect(RemoteDetectionHost, RemoteDetectionPort);
                    return ((IPEndPoint)sock.LocalEndPoint).Address.ToString();
                }
            }
            catch
            {
                // Fallback: iterate interfaces
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                        return addr.ToString();
                }
                return "127.0.0.1";
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = "Status: " + message;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Server Error",
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
