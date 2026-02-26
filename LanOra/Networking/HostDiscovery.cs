using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LanOra.Networking
{
    /// <summary>
    /// Information about a host discovered on the LAN via UDP broadcast.
    /// </summary>
    internal class HostInfo
    {
        public string   HostName  { get; set; }
        public string   IpAddress { get; set; }
        public int      TcpPort   { get; set; }
        public DateTime LastSeen  { get; set; }

        public override string ToString() => string.Format("{0}  ({1})", HostName, IpAddress);
    }

    /// <summary>
    /// Broadcasts host presence via UDP every two seconds so viewers can
    /// discover this machine automatically. Only hostname, IP and TCP port
    /// are included – the PIN is never sent over broadcast.
    /// </summary>
    internal class HostBeacon : IDisposable
    {
        public const int DiscoveryPort      = 5001;
        private const string Prefix         = "LANORA";
        private const int BroadcastInterval = 2000; // ms

        private UdpClient    _udp;
        private Thread       _thread;
        private volatile bool _running;

        /// <summary>Starts broadcasting host info on the LAN.</summary>
        public void Start(string hostName, string ip, int tcpPort)
        {
            if (_running) return;
            _running = true;

            string payload = string.Format("{0}|{1}|{2}|{3}", Prefix, hostName, ip, tcpPort);

            _thread = new Thread(() =>
            {
                try
                {
                    _udp = new UdpClient();
                    _udp.EnableBroadcast = true;
                    IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
                    byte[] data = Encoding.UTF8.GetBytes(payload);
                    while (_running)
                    {
                        try { _udp.Send(data, data.Length, ep); }
                        catch { /* ignore individual send errors */ }
                        Thread.Sleep(BroadcastInterval);
                    }
                }
                catch { /* ignore – stopped */ }
            })
            { IsBackground = true, Name = "HostBeacon" };

            _thread.Start();
        }

        /// <summary>Stops broadcasting.</summary>
        public void Stop()
        {
            _running = false;
            try { _udp?.Close(); } catch { /* ignore */ }
        }

        public void Dispose() => Stop();
    }

    /// <summary>
    /// Listens for UDP host-discovery broadcasts and maintains an up-to-date
    /// list of available hosts. Fires <see cref="HostsUpdated"/> whenever the
    /// list changes.
    /// </summary>
    internal class HostScanner : IDisposable
    {
        public const int DiscoveryPort  = 5001;
        private const int HostExpiryMs  = 10000; // remove after 10 s without broadcast

        private readonly object _lock = new object();
        private readonly Dictionary<string, HostInfo> _hosts =
            new Dictionary<string, HostInfo>(StringComparer.OrdinalIgnoreCase);

        private UdpClient    _udp;
        private Thread       _receiveThread;
        private Thread       _cleanupThread;
        private volatile bool _running;

        /// <summary>Raised (on a background thread) whenever the host list changes.</summary>
        public event Action HostsUpdated;

        /// <summary>Returns a snapshot of the currently visible hosts.</summary>
        public IReadOnlyList<HostInfo> Hosts
        {
            get { lock (_lock) return new List<HostInfo>(_hosts.Values); }
        }

        /// <summary>Starts listening for host broadcasts.</summary>
        public void Start()
        {
            if (_running) return;
            _running = true;

            try
            {
                _udp = new UdpClient(DiscoveryPort) { EnableBroadcast = true };
            }
            catch
            {
                // Port may already be in use (e.g., two viewer instances); degrade gracefully
                _running = false;
                return;
            }

            _receiveThread = new Thread(ReceiveLoop)  { IsBackground = true, Name = "HostScanner" };
            _cleanupThread = new Thread(CleanupLoop)  { IsBackground = true, Name = "HostScannerCleanup" };
            _receiveThread.Start();
            _cleanupThread.Start();
        }

        /// <summary>Stops listening.</summary>
        public void Stop()
        {
            _running = false;
            try { _udp?.Close(); } catch { /* ignore */ }
        }

        public void Dispose() => Stop();

        // ------------------------------------------------------------------ //
        // Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private void ReceiveLoop()
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            while (_running)
            {
                byte[] data;
                try
                {
                    data = _udp.Receive(ref remote);
                }
                catch
                {
                    break; // socket closed
                }

                string   msg  = Encoding.UTF8.GetString(data);
                HostInfo info = ParsePacket(msg);
                if (info == null) continue;

                bool changed;
                lock (_lock)
                {
                    changed = !_hosts.ContainsKey(info.IpAddress);
                    _hosts[info.IpAddress] = info;
                }
                if (changed)
                    HostsUpdated?.Invoke();
            }
        }

        private void CleanupLoop()
        {
            while (_running)
            {
                Thread.Sleep(2000);
                bool changed = false;
                lock (_lock)
                {
                    var expired = new List<string>();
                    foreach (var kv in _hosts)
                        if ((DateTime.UtcNow - kv.Value.LastSeen).TotalMilliseconds > HostExpiryMs)
                            expired.Add(kv.Key);

                    foreach (string key in expired)
                    {
                        _hosts.Remove(key);
                        changed = true;
                    }
                }
                if (changed)
                    HostsUpdated?.Invoke();
            }
        }

        /// <summary>Parses a UDP broadcast packet. Returns null on failure.</summary>
        private static HostInfo ParsePacket(string msg)
        {
            // Expected format: "LANORA|{hostName}|{ip}|{port}"
            try
            {
                string[] p = msg.Split('|');
                if (p.Length != 4 || p[0] != "LANORA") return null;
                return new HostInfo
                {
                    HostName  = p[1],
                    IpAddress = p[2],
                    TcpPort   = int.Parse(p[3]),
                    LastSeen  = DateTime.UtcNow
                };
            }
            catch { return null; }
        }
    }
}
