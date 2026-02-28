using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using LanOra.Monitoring;
using LanOra.Utilities;

namespace LanOra.Networking
{
    /// <summary>
    /// Connects to a running <see cref="ScreenServer"/>, authenticates via PIN,
    /// continuously reads JPEG frames surfaced through <see cref="FrameReceived"/>,
    /// and maintains the heartbeat protocol by replying to each server heartbeat
    /// with a HeartbeatAck packet.
    ///
    /// Thread model:
    ///   • ViewerReceive – single thread for the entire receive + ACK-send loop.
    ///     NetworkStream is full-duplex; since ACK writes and frame reads never
    ///     overlap (ACK is sent only after a heartbeat packet is fully read), the
    ///     same thread can safely alternate reads and writes on the stream.
    /// </summary>
    internal class ScreenClient
    {
        // ------------------------------------------------------------------ //
        // Configuration                                                       //
        // ------------------------------------------------------------------ //

        public const int Port = 5000;

        /// <summary>PIN to send during the authentication handshake.</summary>
        public string Pin { get; set; }

        /// <summary>TCP connection timeout in milliseconds.</summary>
        public int ConnectTimeoutMs { get; set; } = 5000;

        // ------------------------------------------------------------------ //
        // Events (raised on worker thread – marshal to UI with SafeInvoke)   //
        // ------------------------------------------------------------------ //

        public event Action<Bitmap> FrameReceived;
        public event Action<string> StatusChanged;
        public event Action         Disconnected;
        public event Action<string> ErrorOccurred;

        /// <summary>Live performance statistics for the current viewing session.</summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        // Max frame size sanity guard (4 MB)
        private const int MaxFrameSizeBytes = 4 * 1024 * 1024;

        // Receive timeout: must be > HeartbeatIntervalMs (10 s) to avoid false timeouts.
        // 25 s gives a comfortable margin; if nothing arrives in 25 s the server is gone.
        private const int ReceiveTimeoutMs = 25000;

        private TcpClient     _client;
        private Thread        _receiveThread;
        // Single flag is sufficient: no secondary session threads on the client side.
        // Disconnect() sets this false and closes the socket atomically.
        private volatile bool _running;

        // ------------------------------------------------------------------ //
        // Public API                                                          //
        // ------------------------------------------------------------------ //

        /// <summary>Connects to <paramref name="serverIp"/> and starts the receive loop.</summary>
        public void Connect(string serverIp)
        {
            if (_running) return;
            _running = true;

            _receiveThread = new Thread(() => RunReceiveLoop(serverIp))
            {
                IsBackground = true,
                Name         = "ViewerReceive"
            };
            _receiveThread.Start();
        }

        /// <summary>Disconnects from the server and stops the receive loop.</summary>
        public void Disconnect()
        {
            _running = false;
            try { _client?.Close(); } catch { /* ignore */ }
        }

        // ------------------------------------------------------------------ //
        // Receive loop                                                        //
        // ------------------------------------------------------------------ //

        private void RunReceiveLoop(string serverIp)
        {
            try
            {
                RaiseStatus("Connecting to " + serverIp + "…");

                _client = new TcpClient();
                ConfigureSocket(_client);

                // Async connect with configurable timeout
                IAsyncResult ar = _client.BeginConnect(serverIp, Port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeoutMs))
                {
                    _client.Close();
                    RaiseError("Connection timed out.");
                    return;
                }
                _client.EndConnect(ar);

                Performance.Reset();

                // Use the NetworkStream directly for both read and write.
                // This avoids a shared BufferedStream whose internal read-buffer
                // would be invalidated when we switch from reading frames to writing
                // ACK packets (BufferedStream.Write calls FlushRead → Seek, which
                // throws on a non-seekable NetworkStream).
                NetworkStream ns = _client.GetStream();
                using (BinaryReader reader = new BinaryReader(ns, System.Text.Encoding.UTF8, leaveOpen: true))
                using (BinaryWriter writer = new BinaryWriter(ns, System.Text.Encoding.UTF8, leaveOpen: true))
                {
                    // --- PIN authentication ---
                    writer.Write(Pin ?? string.Empty);
                    writer.Flush();

                    bool authenticated = reader.ReadBoolean();
                    if (!authenticated)
                    {
                        RaiseError("Authentication failed – wrong PIN.");
                        return;
                    }

                    Logger.Log("Connection established to " + serverIp);
                    RaiseStatus("Connected – receiving stream…");

                    // --- Packet receive loop ---
                    while (_running)
                    {
                        byte typeVal = reader.ReadByte();
                        int  length  = reader.ReadInt32();

                        if ((PacketType)typeVal == PacketType.Heartbeat)
                        {
                            // Consume any heartbeat payload (currently none)
                            if (length > 0)
                                ReadExactBytes(reader, length);

                            // Reply with HeartbeatAck
                            writer.Write((byte)PacketType.HeartbeatAck);
                            writer.Write(0); // no payload
                            writer.Flush();
                            Logger.Log("Heartbeat ACK sent.");
                            continue;
                        }

                        // PacketType.Frame
                        if (length <= 0 || length > MaxFrameSizeBytes)
                        {
                            RaiseError("Invalid frame length received: " + length);
                            break;
                        }

                        byte[] frameData = ReadExactBytes(reader, length);
                        if (frameData == null) break;

                        Performance.RecordFrame(frameData.Length);

                        Bitmap bmp = DecodeBitmap(frameData);
                        if (bmp != null)
                            FrameReceived?.Invoke(bmp);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                // Host closed connection cleanly
            }
            catch (IOException)
            {
                if (_running)
                    RaiseError("Connection lost.");
            }
            catch (SocketException ex)
            {
                RaiseError("Socket error: " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                // Client was disposed (Disconnect called)
            }
            catch (Exception ex)
            {
                if (_running)
                    RaiseError("Error: " + ex.Message);
            }
            finally
            {
                _running = false;
                try { _client?.Close(); } catch { /* ignore */ }
                Performance.Reset();
                Logger.Log("Disconnected from server.");
                Disconnected?.Invoke();
                RaiseStatus("Disconnected.");
            }
        }

        // ------------------------------------------------------------------ //
        // Helpers                                                             //
        // ------------------------------------------------------------------ //

        /// <summary>Reads exactly <paramref name="count"/> bytes; returns null if stream ends early.</summary>
        private static byte[] ReadExactBytes(BinaryReader reader, int count)
        {
            byte[] buffer = new byte[count];
            int    offset = 0;
            while (offset < count)
            {
                int read = reader.Read(buffer, offset, count - offset);
                if (read == 0) return null; // stream closed
                offset += read;
            }
            return buffer;
        }

        private static Bitmap DecodeBitmap(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                    return new Bitmap(ms);
            }
            catch { return null; }
        }

        /// <summary>
        /// Applies hardened TCP settings to the outgoing socket.
        /// KeepAlive timing uses IOControl (SIO_KEEPALIVE_VALS), supported on
        /// Windows 7 SP1 and later.
        /// </summary>
        private static void ConfigureSocket(TcpClient client)
        {
            client.NoDelay           = true;
            client.ReceiveBufferSize = 1024 * 1024; // 1 MB
            client.SendBufferSize    = 1024 * 1024; // 1 MB
            client.SendTimeout       = 10000;
            // Receive timeout acts as the heartbeat watchdog:
            // if nothing arrives for 25 s (server heartbeat interval is 10 s),
            // the read throws and the session is cleaned up.
            client.ReceiveTimeout    = ReceiveTimeoutMs;

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

        private void RaiseStatus(string msg) => StatusChanged?.Invoke(msg);
        private void RaiseError(string msg)   => ErrorOccurred?.Invoke(msg);
    }
}

