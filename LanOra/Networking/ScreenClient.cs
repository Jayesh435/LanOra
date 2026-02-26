using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using LanOra.Monitoring;

namespace LanOra.Networking
{
    /// <summary>
    /// Connects to a running <see cref="ScreenServer"/>, authenticates via PIN,
    /// and continuously reads JPEG frames surfaced through <see cref="FrameReceived"/>.
    /// Feeds <see cref="Performance"/> with per-frame byte counts for live stats.
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

        /// <summary>
        /// Live performance statistics for the current viewing session.
        /// </summary>
        public PerformanceTracker Performance { get; } = new PerformanceTracker();

        private const int MaxFrameSizeBytes = 4 * 1024 * 1024; // 4 MB sanity check

        private TcpClient    _client;
        private Thread       _receiveThread;
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

                using (BufferedStream buffered = new BufferedStream(_client.GetStream(), 65536))
                using (BinaryWriter   writer   = new BinaryWriter(buffered))
                using (BinaryReader   reader   = new BinaryReader(buffered))
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

                    RaiseStatus("Connected – receiving stream…");

                    // --- Frame receive loop ---
                    while (_running)
                    {
                        int length = reader.ReadInt32(); // 4-byte length prefix

                        if (length <= 0 || length > MaxFrameSizeBytes)
                        {
                            RaiseError("Invalid frame length received.");
                            break;
                        }

                        byte[] frameData = ReadExactBytes(reader, length);
                        if (frameData == null) break;

                        // Record performance stats
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
                Disconnected?.Invoke();
                RaiseStatus("Disconnected.");
            }
        }

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

        private void RaiseStatus(string msg) => StatusChanged?.Invoke(msg);
        private void RaiseError(string msg)   => ErrorOccurred?.Invoke(msg);
    }
}

