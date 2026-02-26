using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LanOra.Networking;

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

        public HostForm()
        {
            InitializeComponent();
            _pin = GeneratePin();
            WireEvents();
            lblIpValue.Text  = GetLocalIpAddress();
            lblPinValue.Text = _pin;
            lblPort.Text     = "Port: " + ScreenServer.Port;
        }

        // ------------------------------------------------------------------ //
        // PIN generation                                                      //
        // ------------------------------------------------------------------ //

        private static string GeneratePin()
        {
            // Cryptographically random 6-digit PIN
            byte[] buf = new byte[4];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(buf);
            uint raw = BitConverter.ToUInt32(buf, 0) % 900000u;
            return (100000u + raw).ToString();
        }

        // ------------------------------------------------------------------ //
        // Event wiring                                                        //
        // ------------------------------------------------------------------ //

        private void WireEvents()
        {
            _server.StatusChanged      += msg => SafeInvoke(() => UpdateStatus(msg));
            _server.ClientConnected    += ip  => SafeInvoke(() => OnClientConnected(ip));
            _server.ClientDisconnected +=       () => SafeInvoke(OnClientDisconnected);
            _server.ErrorOccurred      += msg => SafeInvoke(() => ShowError(msg));
        }

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
                btnStart.Enabled = false;
                btnStop.Enabled  = true;
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
            btnStart.Enabled = true;
            btnStop.Enabled  = false;
            UpdateStatus("Server stopped.");
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
            lblStatusDot.ForeColor = System.Drawing.Color.LimeGreen;
            UpdateStatus("Connected: " + ip);
        }

        private void OnClientDisconnected()
        {
            lblStatusDot.ForeColor = System.Drawing.Color.Red;
            UpdateStatus("Waiting for connection…");
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        private void UpdateStatus(string message) => lblStatus.Text = "Status: " + message;

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
