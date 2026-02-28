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
    /// Wire-protocol packet type.
    /// Every packet on the stream is framed as:
    ///   [PacketType: 1 byte][PayloadLength: 4 bytes][Payload: PayloadLength bytes]
    /// </summary>
    internal enum PacketType : byte
    {
        Frame        = 0,
        Heartbeat    = 1,
        HeartbeatAck = 2
    }

    /// <summary>
    /// Listens for a single viewer connection, authenticates via PIN, streams
    /// compressed JPEG frames, and maintains a heartbeat channel to detect silent
    /// TCP drops before the OS would report them.
    ///
    /// Thread model per session:
    ///   • ServerAccept  – accept loop (one per server lifetime)
    ///   • ServerStream  – frame capture + heartbeat send (HandleClient thread)
    ///   • ServerAckReader – reads HeartbeatAck packets from the viewer
    ///   • ServerMemLog  – logs GC memory every 30 s
    /// </summary>
    internal class ScreenServer
    {
        // ------------------------------------------------------------------ //
        // Public configuration                                                //
        // ------------------------------------------------------------------ //

        /// <summary>Port the server listens on.</summary>
        public const int Port = 5000;

        /// <summary>PIN required for authentication. Set before calling <see cref="Start"/>.</summary>
        public string Pin { get; set; }

        /// <summary>Target frames per second. Default: 10.</summary>
        public int TargetFps
        {
            get { return _targetFps; }
            set { _targetFps = (value > 0) ? value : 10; }
        }

        /// <summary>Resolution preset applied before JPEG compression. Default: HD720p.</summary>
        public ScreenCapture.Resolution ResolutionPreset { get; set; } =
            ScreenCapture.Resolution.HD720p;

        // ------------------------------------------------------------------ //
        // Events (raised on worker threads – use SafeInvoke in UI handlers)  //
        // ------------------------------------------------------------------ //

        public event Action<string> StatusChanged;
        public event Action<string> ClientConnected;
        public event Action         ClientDisconnected;
        public event Action<string> ErrorOccurred;

        /// <summary>Live performance statistics for the current streaming session.</summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private TcpListener   _listener;
        private TcpClient     _currentClient;
        private Thread        _acceptThread;
        private volatile bool _running;
        private volatile bool _isSendingFrame;   // frame-skip flag
        private int           _targetFps = 10;

        // Per-session heartbeat state (Interlocked for 32-bit safety)
        private volatile bool _sessionRunning;
        private long          _lastAckTicks;     // last HeartbeatAck tick (Stopwatch)

        // Heartbeat timing constants
        private const int HeartbeatIntervalMs = 10000;  // send every 10 s
        private const int HeartbeatTimeoutMs  = 20000;  // disconnect if no ACK for 20 s

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

            Logger.Log("Server started on port " + Port);
            RaiseStatus("Waiting for connection…");
        }

        /// <summary>Stops the server and closes any active connection.</summary>
        public void Stop()
        {
            _running        = false;
            _sessionRunning = false;
            try { _listener?.Stop(); }       catch { /* ignore */ }
            try { _currentClient?.Close(); } catch { /* ignore */ }
            Performance.Reset();
            Logger.Log("Server stopped.");
            RaiseStatus("Stopped.");
        }

        // ------------------------------------------------------------------ //
        // Accept loop                                                         //
        // ------------------------------------------------------------------ //

        private void AcceptLoop()
        {
            while (_running)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    ConfigureSocket(client);
                    _currentClient = client;
                    HandleClient(client);
                }
                catch (SocketException)
                {
                    break; // Stop() closed the listener
                }
                catch (Exception ex)
                {
                    if (_running)
                        RaiseError("Accept error: " + ex.Message);
                }
            }
        }

        // ------------------------------------------------------------------ //
        // Client session                                                      //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Authenticates the viewer, then runs the frame-streaming loop.
        /// Streams are set up so the write thread (this thread) and the ACK-reader
        /// thread operate on separate halves of the full-duplex TCP socket.
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            string clientAddress = "unknown";
            try
            {
                clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                NetworkStream ns = client.GetStream();

                // Writer wraps a dedicated BufferedStream so large frame payloads
                // are assembled in memory before hitting the kernel send buffer.
                // Only the stream thread (this thread) ever calls writer methods.
                BinaryWriter writer = new BinaryWriter(new BufferedStream(ns, 1024 * 1024));

                // Reader reads directly from the NetworkStream.  NetworkStream is
                // full-duplex: concurrent read (ACK thread) + write (this thread) is safe.
                BinaryReader reader = new BinaryReader(ns);

                // --- PIN authentication ---
                string receivedPin   = reader.ReadString();
                bool   authenticated = receivedPin == Pin;
                writer.Write(authenticated);
                writer.Flush();

                if (!authenticated)
                {
                    Logger.Log("Auth failed from " + clientAddress);
                    RaiseStatus("Auth failed – wrong PIN from " + clientAddress);
                    return;
                }

                Logger.Log("Connection established: " + clientAddress);
                ClientConnected?.Invoke(clientAddress);
                RaiseStatus("Connected: " + clientAddress);
                Performance.Reset();

                // Initialise per-session heartbeat state
                _sessionRunning = true;
                Interlocked.Exchange(ref _lastAckTicks, Stopwatch.GetTimestamp());

                // ACK-reader thread: reads HeartbeatAck packets from the viewer.
                new Thread(() => AckReaderLoop(client, reader))
                    { IsBackground = true, Name = "ServerAckReader" }.Start();

                // Memory-logging thread: logs GC heap size every 30 s.
                new Thread(MemoryLogLoop)
                    { IsBackground = true, Name = "ServerMemLog" }.Start();

                // Stream + heartbeat loop runs on this thread (sole writer).
                StreamLoop(client, writer);
            }
            catch (IOException)             { /* viewer disconnected normally */ }
            catch (SocketException ex)      { if (_running) RaiseError("Socket error: " + ex.Message); }
            catch (ObjectDisposedException) { /* session ended */ }
            catch (Exception ex)            { if (_running) RaiseError("Stream error: " + ex.Message); }
            finally
            {
                _sessionRunning = false;
                _isSendingFrame = false;
                try { client.Close(); } catch { /* ignore */ }
                _currentClient = null;
                ClientDisconnected?.Invoke();
                if (_running)
                    RaiseStatus("Waiting for connection…");
                Logger.Log("Client disconnected: " + clientAddress);
            }
        }

        // ------------------------------------------------------------------ //
        // Stream + heartbeat loop (sole writer thread)                       //
        // ------------------------------------------------------------------ //

        private void StreamLoop(TcpClient client, BinaryWriter writer)
        {
            Stopwatch frameTimer     = Stopwatch.StartNew();
            Stopwatch heartbeatTimer = Stopwatch.StartNew();
            long      ticksPerFrame  = Math.Max(1L, Stopwatch.Frequency / _targetFps);
            long      nextFrameTick  = frameTimer.ElapsedTicks + ticksPerFrame;

            while (_running && _sessionRunning)
            {
                // Wait until it is time for the next frame slot.
                while (frameTimer.ElapsedTicks < nextFrameTick)
                    Thread.Sleep(1);

                // Heartbeat: check interval and ACK timeout.
                if (heartbeatTimer.ElapsedMilliseconds >= HeartbeatIntervalMs)
                {
                    long   lastAck    = Interlocked.Read(ref _lastAckTicks);
                    double msSinceAck = (Stopwatch.GetTimestamp() - lastAck)
                                        * 1000.0 / Stopwatch.Frequency;

                    if (msSinceAck > HeartbeatTimeoutMs)
                    {
                        Logger.Log(string.Format(
                            "Heartbeat ACK timeout ({0:F0} ms). Disconnecting.", msSinceAck));
                        _sessionRunning = false;
                        try { client.Close(); } catch { /* ignore */ }
                        break;
                    }

                    try
                    {
                        SendPacket(writer, PacketType.Heartbeat, new byte[0]);
                        Logger.Log("Heartbeat sent.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Heartbeat send failed: " + ex.Message);
                        _sessionRunning = false;
                        break;
                    }

                    heartbeatTimer.Restart();
                }

                // Frame-skip: if previous send is still in progress, skip this slot.
                if (_isSendingFrame)
                {
                    nextFrameTick += ticksPerFrame;
                    continue;
                }

                byte[] frame = ScreenCapture.CaptureScreen(ResolutionPreset);
                if (frame != null && frame.Length > 0)
                {
                    _isSendingFrame = true;
                    try
                    {
                        SendPacket(writer, PacketType.Frame, frame);
                        Performance.RecordFrame(frame.Length);
                    }
                    catch (IOException)             { _sessionRunning = false; break; }
                    catch (SocketException)         { _sessionRunning = false; break; }
                    catch (ObjectDisposedException) { _sessionRunning = false; break; }
                    catch (Exception ex)
                    {
                        Logger.Log("Frame send failure: " + ex.Message);
                        _sessionRunning = false;
                        break;
                    }
                    finally
                    {
                        _isSendingFrame = false;
                    }
                }

                nextFrameTick += ticksPerFrame;
            }
        }

        // ------------------------------------------------------------------ //
        // ACK-reader loop (dedicated thread)                                 //
        // ------------------------------------------------------------------ //

        private void AckReaderLoop(TcpClient client, BinaryReader reader)
        {
            while (_running && _sessionRunning)
            {
                try
                {
                    byte typeVal = reader.ReadByte();
                    int  length  = reader.ReadInt32();
                    if (length > 0)
                        ReadExactBytes(reader, length); // consume any payload

                    if ((PacketType)typeVal == PacketType.HeartbeatAck)
                    {
                        Interlocked.Exchange(ref _lastAckTicks, Stopwatch.GetTimestamp());
                        Logger.Log("Heartbeat ACK received.");
                    }
                }
                catch (IOException)             { break; }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    Logger.Log("ACK read error: " + ex.Message);
                    break;
                }
            }
        }

        // ------------------------------------------------------------------ //
        // Memory-logging loop                                                 //
        // ------------------------------------------------------------------ //

        private void MemoryLogLoop()
        {
            while (_sessionRunning)
            {
                Thread.Sleep(30000);
                if (_sessionRunning)
                    Logger.LogMemory();
            }
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Writes a framed packet: [type: 1 byte][length: 4 bytes][payload].
        /// Must only be called from the sole writer thread.
        /// </summary>
        private static void SendPacket(BinaryWriter writer, PacketType type, byte[] payload)
        {
            writer.Write((byte)type);
            writer.Write(payload.Length);
            if (payload.Length > 0)
                writer.Write(payload);
            writer.Flush();
        }

        /// <summary>Reads exactly <paramref name="count"/> bytes; returns null if stream ends early.</summary>
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

        /// <summary>
        /// Applies hardened TCP settings to a newly accepted client socket.
        /// KeepAlive timing uses IOControl (SIO_KEEPALIVE_VALS), supported on
        /// Windows 7 SP1 and later.
        /// </summary>
        private static void ConfigureSocket(TcpClient client)
        {
            client.NoDelay           = true;
            client.ReceiveBufferSize = 1024 * 1024; // 1 MB
            client.SendBufferSize    = 1024 * 1024; // 1 MB
            client.SendTimeout       = 10000;
            client.ReceiveTimeout    = 0; // heartbeat logic handles dead connections

            // Enable TCP KeepAlive at the socket level
            client.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.KeepAlive,
                true);

            // SIO_KEEPALIVE_VALS layout: [onoff: 4 B][idle time: 4 B][interval: 4 B] (ms)
            byte[] ka = new byte[12];
            BitConverter.GetBytes(1u).CopyTo(ka, 0);      // enable
            BitConverter.GetBytes(30000u).CopyTo(ka, 4);  // idle before first probe: 30 s
            BitConverter.GetBytes(10000u).CopyTo(ka, 8);  // retry interval: 10 s
            client.Client.IOControl(IOControlCode.KeepAliveValues, ka, null);
        }

        private void RaiseStatus(string message) => StatusChanged?.Invoke(message);
        private void RaiseError(string message)   => ErrorOccurred?.Invoke(message);
    }
}

