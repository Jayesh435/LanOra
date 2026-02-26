using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LanOra.Monitoring;
using LanOra.Utilities;

namespace LanOra.Networking
{
    /// <summary>
    /// Listens for a single viewer connection, authenticates the client via PIN,
    /// and continuously streams compressed screen frames over TCP.
    ///
    /// Frame pacing is Stopwatch-based to avoid Thread.Sleep drift.
    /// Frame skipping ensures the network is never over-queued.
    /// </summary>
    internal class ScreenServer
    {
        // ------------------------------------------------------------------ //
        // Public configuration                                                //
        // ------------------------------------------------------------------ //

        /// <summary>Port the server listens on.</summary>
        public const int Port = 5000;

        /// <summary>
        /// PIN required for authentication. Set this before calling
        /// <see cref="Start"/>.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Target frames per second. Supported values: 5, 10, 15, 30.
        /// Default: 10.
        /// </summary>
        public int TargetFps
        {
            get { return _targetFps; }
            set { _targetFps = (value > 0) ? value : 10; }
        }

        /// <summary>
        /// Resolution preset applied before JPEG compression.
        /// Default: HD720p (1280 × 720).
        /// </summary>
        public ScreenCapture.Resolution ResolutionPreset { get; set; } =
            ScreenCapture.Resolution.HD720p;

        // ------------------------------------------------------------------ //
        // Events (raised on worker threads – use SafeInvoke in the UI)       //
        // ------------------------------------------------------------------ //

        public event Action<string> StatusChanged;
        public event Action<string> ClientConnected;
        public event Action         ClientDisconnected;
        public event Action<string> ErrorOccurred;

        /// <summary>
        /// Live performance statistics for the currently streaming session.
        /// </summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private TcpListener   _listener;
        private TcpClient     _currentClient;
        private Thread        _acceptThread;
        private volatile bool _running;
        private volatile bool _clientBusy;   // frame-skip flag
        private int           _targetFps = 10;

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
            try { _listener?.Stop(); }       catch { /* ignore */ }
            try { _currentClient?.Close(); } catch { /* ignore */ }
            Performance.Reset();
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
                    client.NoDelay        = true;
                    client.ReceiveTimeout = 5000;
                    client.SendTimeout    = 5000;
                    _currentClient = client;
                    HandleClient(client);
                }
                catch (SocketException)
                {
                    break; // Raised when Stop() closes the listener
                }
                catch (Exception ex)
                {
                    if (_running)
                        RaiseError("Accept error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Authenticates the viewer via PIN, then streams frames until the
        /// connection drops or the server is stopped.
        /// Uses Stopwatch-based frame pacing to avoid Thread.Sleep drift.
        /// Skips a frame if the previous send is still in progress.
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            string clientAddress = "unknown";
            try
            {
                clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                using (BufferedStream buffered = new BufferedStream(client.GetStream(), 65536))
                using (BinaryReader   reader   = new BinaryReader(buffered))
                using (BinaryWriter   writer   = new BinaryWriter(buffered))
                {
                    // --- PIN authentication handshake ---
                    string receivedPin   = reader.ReadString();
                    bool   authenticated = receivedPin == Pin;
                    writer.Write(authenticated);
                    writer.Flush();

                    if (!authenticated)
                    {
                        RaiseStatus("Auth failed – wrong PIN from " + clientAddress);
                        return;
                    }

                    ClientConnected?.Invoke(clientAddress);
                    RaiseStatus("Connected: " + clientAddress);
                    Performance.Reset();

                    // --- Stopwatch-based frame pacing ---
                    Stopwatch sw             = Stopwatch.StartNew();
                    long      ticksPerFrame  = Math.Max(1L, Stopwatch.Frequency / _targetFps);
                    long      nextFrameTick  = sw.ElapsedTicks + ticksPerFrame;

                    while (_running)
                    {
                        // Wait until it is time for the next frame.
                        // Thread.Sleep(1) yields the CPU while still waking up
                        // frequently enough for typical frame intervals (33–200 ms).
                        while (sw.ElapsedTicks < nextFrameTick)
                            Thread.Sleep(1);

                        // Frame-skip: if a send is still busy, skip this slot
                        if (_clientBusy)
                        {
                            nextFrameTick += ticksPerFrame;
                            continue;
                        }

                        byte[] frame = ScreenCapture.CaptureScreen(ResolutionPreset);
                        if (frame != null && frame.Length > 0)
                        {
                            _clientBusy = true;
                            try
                            {
                                writer.Write(frame.Length); // 4-byte length prefix
                                writer.Write(frame);        // JPEG bytes
                                writer.Flush();
                                Performance.RecordFrame(frame.Length);
                            }
                            finally
                            {
                                _clientBusy = false;
                            }
                        }

                        nextFrameTick += ticksPerFrame;
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
                _clientBusy    = false;
                ClientDisconnected?.Invoke();
                if (_running)
                    RaiseStatus("Waiting for connection…");
            }
        }

        private void RaiseStatus(string message) => StatusChanged?.Invoke(message);
        private void RaiseError(string message)   => ErrorOccurred?.Invoke(message);
    }
}

