using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LANMonitor.Server.Utilities;

namespace LANMonitor.Server.Networking
{
    /// <summary>
    /// Listens for a single viewer connection, authenticates the client, and
    /// continuously streams compressed screen frames via TCP.
    /// </summary>
    internal class ScreenServer
    {
        // ------------------------------------------------------------------ //
        // Public configuration                                                //
        // ------------------------------------------------------------------ //

        /// <summary>Port the server listens on.</summary>
        public const int Port = 5000;

        /// <summary>Shared password. Change before deployment.</summary>
        public const string Password = "lanora123";

        /// <summary>Milliseconds between captured frames.</summary>
        public int CaptureIntervalMs { get; set; } = 100;

        // ------------------------------------------------------------------ //
        // Events (raised on worker threads – use Invoke on UI)               //
        // ------------------------------------------------------------------ //

        public event Action<string> StatusChanged;
        public event Action<string> ClientConnected;
        public event Action         ClientDisconnected;
        public event Action<string> ErrorOccurred;

        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private TcpListener  _listener;
        private TcpClient    _currentClient;
        private Thread       _acceptThread;
        private volatile bool _running;

        // ------------------------------------------------------------------ //
        // Public API                                                          //
        // ------------------------------------------------------------------ //

        /// <summary>Starts listening for incoming viewer connections.</summary>
        public void Start()
        {
            if (_running) return;

            _running  = true;
            _listener = new TcpListener(IPAddress.Any, Port);
            _listener.Start();

            _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "ServerAccept" };
            _acceptThread.Start();

            RaiseStatus("Waiting for connection…");
        }

        /// <summary>Stops the server and closes any active connection.</summary>
        public void Stop()
        {
            _running = false;

            try { _listener?.Stop(); }         catch { /* ignore */ }
            try { _currentClient?.Close(); }   catch { /* ignore */ }

            RaiseStatus("Stopped.");
        }

        // ------------------------------------------------------------------ //
        // Private helpers                                                     //
        // ------------------------------------------------------------------ //

        /// <summary>Blocking loop – waits for one client at a time.</summary>
        private void AcceptLoop()
        {
            while (_running)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();

                    // Configure TCP keep-alive & no-delay for lower latency
                    client.NoDelay       = true;
                    client.ReceiveTimeout = 5000;
                    client.SendTimeout    = 5000;

                    _currentClient = client;
                    HandleClient(client);
                }
                catch (SocketException)
                {
                    // Raised when Stop() calls _listener.Stop() – exit loop
                    break;
                }
                catch (Exception ex)
                {
                    if (_running)
                        RaiseError("Accept error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Authenticates, then streams frames until the connection drops or
        /// the server is stopped.
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            string clientAddress = "unknown";

            try
            {
                clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                using (BufferedStream buffered = new BufferedStream(client.GetStream(), 65536))
                using (BinaryReader reader = new BinaryReader(buffered))
                using (BinaryWriter writer = new BinaryWriter(buffered))
                {
                    // --- Authentication handshake ---
                    string receivedPassword = reader.ReadString();
                    bool   authenticated    = receivedPassword == Password;
                    writer.Write(authenticated);
                    writer.Flush();

                    if (!authenticated)
                    {
                        RaiseStatus("Auth failed – wrong password from " + clientAddress);
                        return;
                    }

                    ClientConnected?.Invoke(clientAddress);
                    RaiseStatus("Connected: " + clientAddress);

                    // --- Streaming loop (rely on exceptions for disconnect detection) ---
                    while (_running)
                    {
                        byte[] frame = ScreenCapture.CaptureScreen();

                        if (frame != null && frame.Length > 0)
                        {
                            writer.Write(frame.Length);  // 4-byte length prefix
                            writer.Write(frame);         // JPEG bytes
                            writer.Flush();
                        }

                        Thread.Sleep(CaptureIntervalMs);
                    }
                }
            }
            catch (IOException)
            {
                // Normal – viewer disconnected
            }
            catch (Exception ex)
            {
                if (_running)
                    RaiseError("Stream error: " + ex.Message);
            }
            finally
            {
                try { client.Close(); } catch { /* ignore */ }
                _currentClient = null;

                ClientDisconnected?.Invoke();

                if (_running)
                    RaiseStatus("Waiting for connection…");
            }
        }

        private void RaiseStatus(string message)  => StatusChanged?.Invoke(message);
        private void RaiseError(string message)    => ErrorOccurred?.Invoke(message);
    }
}
