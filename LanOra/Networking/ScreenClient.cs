using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LanOra.Input;
using LanOra.Monitoring;

namespace LanOra.Networking
{
    /// <summary>
    /// Connects to a running <see cref="ScreenServer"/>, authenticates via PIN,
    /// and continuously reads JPEG frames surfaced through <see cref="FrameReceived"/>.
    ///
    /// Protocol (post-authentication):
    ///   [1 byte PacketType][4 bytes PayloadLength][Payload]
    ///
    /// Threading model:
    ///   • RunReceiveLoop thread – reads server packets (Frame, ControlResponse,
    ///                             ControlRelease).
    ///   • All outgoing packets (ControlRequest, MouseEvent, KeyboardEvent,
    ///     ControlRelease) are written by the caller under _writeLock so that
    ///     no two threads write simultaneously.
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

        /// <summary>Raised when the host approves the control request.</summary>
        public event Action ControlApproved;

        /// <summary>Raised when the host denies the control request.</summary>
        public event Action ControlDenied;

        /// <summary>Raised when the host revokes control (or the connection drops).</summary>
        public event Action ControlRevoked;

        /// <summary>
        /// Live performance statistics for the current viewing session.
        /// </summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private const int MaxFrameSizeBytes = 4 * 1024 * 1024; // 4 MB sanity check

        private TcpClient     _client;
        private Thread        _receiveThread;
        private volatile bool _running;

        // Writer shared between the UI thread (send input) and itself; guarded by _writeLock.
        // Only the _receiveThread reads; this writer is only written to.
        private BinaryWriter  _writer;
        private readonly object _writeLock = new object();

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

        /// <summary>
        /// Sends a ControlRequest to the host.
        /// Payload: UTF-8 encoded "MachineName|IP".
        /// </summary>
        public void SendControlRequest(string viewerInfo)
        {
            SendPacketLocked(PacketType.ControlRequest, Encoding.UTF8.GetBytes(viewerInfo));
        }

        /// <summary>Sends a ControlRelease packet to the host (viewer-initiated release).</summary>
        public void SendControlRelease()
        {
            SendPacketLocked(PacketType.ControlRelease, new byte[0]);
        }

        /// <summary>Sends a mouse or keyboard input packet to the host.</summary>
        public void SendInputPacket(InputPacket packet)
        {
            PacketType type = (packet.EventType == InputEventType.KeyDown ||
                               packet.EventType == InputEventType.KeyUp)
                              ? PacketType.KeyboardEvent
                              : PacketType.MouseEvent;

            SendPacketLocked(type, packet.Serialize());
        }

        // ------------------------------------------------------------------ //
        // Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private void RunReceiveLoop(string serverIp)
        {
            try
            {
                RaiseStatus("Connecting to " + serverIp + "…");

                _client = new TcpClient
                {
                    NoDelay        = true,
                    ReceiveTimeout = 10000,
                    SendTimeout    = 5000
                };

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

                NetworkStream ns     = _client.GetStream();
                BinaryWriter  writer = new BinaryWriter(new BufferedStream(ns, 65536));
                BinaryReader  reader = new BinaryReader(ns);

                lock (_writeLock) { _writer = writer; }

                // --- PIN authentication ---
                // Authentication uses the pre-protocol legacy format (plain string + bool)
                // to match the server's handshake code.
                lock (_writeLock)
                {
                    writer.Write(Pin ?? string.Empty);
                    writer.Flush();
                }

                bool authenticated = reader.ReadBoolean();
                if (!authenticated)
                {
                    RaiseError("Authentication failed – wrong PIN.");
                    return;
                }

                RaiseStatus("Connected – receiving stream…");

                // --- Packet receive loop ---
                while (_running)
                {
                    PacketType type       = (PacketType)reader.ReadByte();
                    int        payloadLen = reader.ReadInt32();

                    if (payloadLen < 0 || payloadLen > MaxFrameSizeBytes)
                    {
                        RaiseError("Invalid payload length received.");
                        break;
                    }

                    byte[] payload = payloadLen > 0
                                     ? ReadExactBytes(reader, payloadLen)
                                     : new byte[0];

                    if (payload == null) break; // stream ended

                    switch (type)
                    {
                        case PacketType.Frame:
                            Performance.RecordFrame(payload.Length);
                            Bitmap bmp = DecodeBitmap(payload);
                            if (bmp != null)
                                FrameReceived?.Invoke(bmp);
                            break;

                        case PacketType.ControlResponse:
                            bool approved = payload.Length > 0 && payload[0] == 1;
                            if (approved) ControlApproved?.Invoke();
                            else          ControlDenied?.Invoke();
                            break;

                        case PacketType.ControlRelease:
                            ControlRevoked?.Invoke();
                            break;
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
            catch (Exception ex)
            {
                if (_running)
                    RaiseError("Error: " + ex.Message);
            }
            finally
            {
                _running = false;
                try { _client?.Close(); } catch { /* ignore */ }
                lock (_writeLock) { _writer = null; }
                Performance.Reset();
                Disconnected?.Invoke();
                RaiseStatus("Disconnected.");
            }
        }

        /// <summary>
        /// Acquires _writeLock and writes a complete packet to the server.
        /// Safe to call from any thread.
        /// </summary>
        private void SendPacketLocked(PacketType type, byte[] payload)
        {
            if (!_running) return;
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

        private static Bitmap DecodeBitmap(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                    return new Bitmap(ms);
            }
            catch { return null; }
        }

        private void RaiseStatus(string msg) => StatusChanged?.Invoke(msg);
        private void RaiseError(string msg)   => ErrorOccurred?.Invoke(msg);
    }
}

