using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LanOra.Input;
using LanOra.Monitoring;
using LanOra.Security;
using LanOra.Utilities;

namespace LanOra.Networking
{
    /// <summary>
    /// Listens for a single viewer connection, authenticates via PIN, and
    /// continuously streams compressed screen frames over TCP.
    ///
    /// Protocol (post-authentication):
    ///   [1 byte PacketType][4 bytes PayloadLength][Payload]
    ///
    /// Threading model:
    ///   • AcceptLoop thread    – blocks on TcpListener.AcceptTcpClient().
    ///   • Frame-send thread    – the AcceptLoop thread after a client connects;
    ///                            writes Frame packets in a Stopwatch-paced loop.
    ///   • Input-receive thread – started alongside the frame-send loop;
    ///                            reads ControlRequest / MouseEvent / KeyboardEvent
    ///                            / ControlRelease packets from the viewer.
    ///
    ///   Only one thread writes to the TCP stream at a time (guarded by
    ///   _writeLock).  Only one thread reads (the input-receive thread).
    ///   NetworkStream supports concurrent reads and writes safely.
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
        /// Raised when the viewer sends a ControlRequest.
        /// Argument is "MachineName|IP" sent by the viewer.
        /// </summary>
        public event Action<string> ControlRequestReceived;

        /// <summary>Raised when the viewer sends ControlRelease.</summary>
        public event Action ControlReleased;

        /// <summary>
        /// Raised each time a valid input event is injected (for the activity
        /// flash indicator in the host UI).
        /// </summary>
        public event Action InputActivityFlash;

        /// <summary>
        /// Live performance statistics for the currently streaming session.
        /// </summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        // ------------------------------------------------------------------ //
        // Dependencies (set by HostForm after construction)                  //
        // ------------------------------------------------------------------ //

        /// <summary>Shared control-state tracker. Set by HostForm.</summary>
        public ControlManager ControlManager { get; set; }

        /// <summary>Session logger. Set by HostForm.</summary>
        public SessionLogger  Logger         { get; set; }

        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private TcpListener    _listener;
        private TcpClient      _currentClient;
        private Thread         _acceptThread;
        private volatile bool  _running;
        private int            _targetFps = 10;

        // Shared writer – guarded by _writeLock.
        // Both the frame-send loop and SendControlResponse write to this writer.
        private BinaryWriter   _writer;
        private readonly object _writeLock = new object();

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

        /// <summary>
        /// Sends a ControlResponse packet to the connected viewer.
        /// Thread-safe – may be called from any thread (e.g. UI after dialog).
        /// </summary>
        public void SendControlResponse(bool approved)
        {
            byte[] payload = new byte[] { approved ? (byte)1 : (byte)0 };
            SendPacketLocked(PacketType.ControlResponse, payload);
        }

        /// <summary>
        /// Sends a ControlRelease packet to the viewer (host-initiated revoke).
        /// </summary>
        public void SendControlRevoke()
        {
            SendPacketLocked(PacketType.ControlRelease, new byte[0]);
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
        ///
        /// A concurrent InputReceiveLoop thread reads viewer-to-host packets
        /// on the same TCP connection (NetworkStream is full-duplex).
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            string clientAddress = "unknown";
            Thread inputThread   = null;

            try
            {
                clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                NetworkStream ns = client.GetStream();

                // Writer is buffered for frame efficiency; guarded by _writeLock.
                // Reader wraps the raw stream – only InputReceiveLoop reads.
                BinaryWriter writer = new BinaryWriter(new BufferedStream(ns, 65536));
                BinaryReader reader = new BinaryReader(ns);

                lock (_writeLock) { _writer = writer; }

                // --- PIN authentication handshake ---
                string receivedPin   = reader.ReadString();
                bool   authenticated = receivedPin == Pin;

                lock (_writeLock)
                {
                    writer.Write(authenticated);
                    writer.Flush();
                }

                if (!authenticated)
                {
                    RaiseStatus("Auth failed – wrong PIN from " + clientAddress);
                    return;
                }

                ClientConnected?.Invoke(clientAddress);
                RaiseStatus("Connected: " + clientAddress);
                Logger?.Log("Viewer Connected: " + clientAddress);
                ControlManager?.SetConnected(true);
                Performance.Reset();

                // Start concurrent input-receive thread
                inputThread = new Thread(() => InputReceiveLoop(reader, clientAddress))
                {
                    IsBackground = true,
                    Name         = "ServerInputRecv"
                };
                inputThread.Start();

                // --- Stopwatch-based frame-pacing loop ---
                Stopwatch sw            = Stopwatch.StartNew();
                long      ticksPerFrame = Math.Max(1L, Stopwatch.Frequency / _targetFps);
                long      nextFrameTick = sw.ElapsedTicks + ticksPerFrame;

                while (_running)
                {
                    while (sw.ElapsedTicks < nextFrameTick)
                        Thread.Sleep(1);

                    byte[] frame = ScreenCapture.CaptureScreen(ResolutionPreset);
                    if (frame != null && frame.Length > 0)
                    {
                        // TryEnter: skip this frame if a ControlResponse is
                        // being written concurrently rather than blocking.
                        if (Monitor.TryEnter(_writeLock))
                        {
                            try
                            {
                                writer.Write((byte)PacketType.Frame);
                                writer.Write(frame.Length);
                                writer.Write(frame);
                                writer.Flush();
                                Performance.RecordFrame(frame.Length);
                            }
                            finally
                            {
                                Monitor.Exit(_writeLock);
                            }
                        }
                        // else: skip frame – will retry at the next tick
                    }

                    nextFrameTick += ticksPerFrame;
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

                // Give the input thread a moment to exit cleanly
                try { inputThread?.Join(2000); } catch { /* ignore */ }

                lock (_writeLock) { _writer = null; }
                _currentClient = null;

                ControlManager?.SetConnected(false);
                Logger?.Log("Session Ended");

                ClientDisconnected?.Invoke();
                if (_running)
                    RaiseStatus("Waiting for connection…");
            }
        }

        /// <summary>
        /// Runs on a dedicated background thread.  Reads packets sent by the
        /// viewer and dispatches them.
        /// </summary>
        private void InputReceiveLoop(BinaryReader reader, string clientAddress)
        {
            try
            {
                while (_running)
                {
                    PacketType type       = (PacketType)reader.ReadByte();
                    int        payloadLen = reader.ReadInt32();
                    byte[]     payload    = payloadLen > 0
                                            ? ReadExactBytes(reader, payloadLen)
                                            : new byte[0];

                    if (payload == null) break; // stream ended

                    switch (type)
                    {
                        case PacketType.ControlRequest:
                            Logger?.Log("Control Requested");
                            ControlRequestReceived?.Invoke(Encoding.UTF8.GetString(payload));
                            break;

                        case PacketType.MouseEvent:
                        case PacketType.KeyboardEvent:
                            if (ControlManager != null && ControlManager.IsControlActive)
                            {
                                try
                                {
                                    InputPacket pkt = InputPacket.Deserialize(payload);
                                    InputInjector.InjectFromPacket(pkt);
                                    InputActivityFlash?.Invoke();
                                }
                                catch { /* malformed packet – ignore */ }
                            }
                            else
                            {
                                Logger?.Log("Input rejected – control not active from " + clientAddress);
                            }
                            break;

                        case PacketType.ControlRelease:
                            Logger?.Log("Control Released by viewer");
                            ControlReleased?.Invoke();
                            break;
                    }
                }
            }
            catch (EndOfStreamException)     { /* viewer disconnected */ }
            catch (IOException)               { /* viewer disconnected */ }
            catch (ObjectDisposedException)   { /* stream closed on Stop */ }
            catch (Exception ex)
            {
                if (_running)
                    RaiseError("Input receive error: " + ex.Message);
            }
        }

        /// <summary>
        /// Acquires _writeLock and sends a complete packet.
        /// Safe to call from any thread.
        /// </summary>
        private void SendPacketLocked(PacketType type, byte[] payload)
        {
            lock (_writeLock)
            {
                if (_writer == null) return;
                try
                {
                    _writer.Write((byte)type);
                    _writer.Write(payload.Length);
                    if (payload.Length > 0)
                        _writer.Write(payload);
                    _writer.Flush();
                }
                catch { /* connection may have dropped */ }
            }
        }

        /// <summary>Reads exactly <paramref name="count"/> bytes; returns null if the stream ends.</summary>
        private static byte[] ReadExactBytes(BinaryReader reader, int count)
        {
            byte[] buffer = new byte[count];
            int    offset = 0;
            while (offset < count)
            {
                int read = reader.Read(buffer, offset, count - offset);
                if (read == 0) return null;
                offset += read;
            }
            return buffer;
        }

        private void RaiseStatus(string message) => StatusChanged?.Invoke(message);
        private void RaiseError(string message)   => ErrorOccurred?.Invoke(message);
    }
}

