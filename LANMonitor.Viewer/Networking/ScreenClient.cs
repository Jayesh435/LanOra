using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace LANMonitor.Viewer.Networking
{
    /// <summary>
    /// Connects to a running ScreenServer, authenticates, and continuously
    /// reads JPEG frames which are surfaced through the <see cref="FrameReceived"/>
    /// event for display in the UI.
    /// </summary>
    internal class ScreenClient
    {
        // ------------------------------------------------------------------ //
        // Configuration                                                       //
        // ------------------------------------------------------------------ //

        public const int    Port     = 5000;
        public const string Password = "lanora123";

        /// <summary>TCP connection timeout in milliseconds.</summary>
        public int ConnectTimeoutMs { get; set; } = 5000;

        // ------------------------------------------------------------------ //
        // Events (raised on worker thread – use Invoke on UI)                //
        // ------------------------------------------------------------------ //

        public event Action<Bitmap> FrameReceived;
        public event Action<string> StatusChanged;
        public event Action         Disconnected;
        public event Action<string> ErrorOccurred;

        // Maximum accepted frame size (10 MB) to guard against corrupt data
        private const int MaxFrameSizeBytes = 10 * 1024 * 1024;

        private TcpClient    _client;
        private Thread       _receiveThread;
        private volatile bool _running;

        // ------------------------------------------------------------------ //
        // Public API                                                          //
        // ------------------------------------------------------------------ //

        /// <summary>Connects to the server at <paramref name="serverIp"/> and starts streaming.</summary>
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

                _client = new TcpClient();
                _client.NoDelay       = true;
                _client.ReceiveTimeout = 10000;
                _client.SendTimeout    = 5000;

                // Async connect with timeout
                IAsyncResult ar = _client.BeginConnect(serverIp, Port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeoutMs))
                {
                    _client.Close();
                    RaiseError("Connection timed out.");
                    return;
                }
                _client.EndConnect(ar);

                using (BufferedStream buffered = new BufferedStream(_client.GetStream(), 65536))
                using (BinaryWriter writer = new BinaryWriter(buffered))
                using (BinaryReader reader = new BinaryReader(buffered))
                {
                    // --- Authentication ---
                    writer.Write(Password);
                    writer.Flush();

                    bool authenticated = reader.ReadBoolean();
                    if (!authenticated)
                    {
                        RaiseError("Authentication failed – wrong password.");
                        return;
                    }

                    RaiseStatus("Connected – receiving stream…");

                    // --- Frame receive loop ---
                    while (_running)
                    {
                        int length = reader.ReadInt32();     // 4-byte length prefix

                        if (length <= 0 || length > MaxFrameSizeBytes) // sanity check
                        {
                            RaiseError("Invalid frame length received.");
                            break;
                        }

                        byte[] frameData = ReadExactBytes(reader, length);
                        if (frameData == null) break;

                        Bitmap bmp = DecodeBitmap(frameData);
                        if (bmp != null)
                            FrameReceived?.Invoke(bmp);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                // Server closed connection cleanly
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
                Disconnected?.Invoke();
                RaiseStatus("Disconnected.");
            }
        }

        /// <summary>
        /// Reads exactly <paramref name="count"/> bytes from the stream.
        /// Returns null if the stream ends prematurely.
        /// </summary>
        private static byte[] ReadExactBytes(BinaryReader reader, int count)
        {
            byte[] buffer  = new byte[count];
            int    offset  = 0;

            while (offset < count)
            {
                int read = reader.Read(buffer, offset, count - offset);
                if (read == 0)
                    return null; // Stream closed
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
            catch
            {
                return null;
            }
        }

        private void RaiseStatus(string msg) => StatusChanged?.Invoke(msg);
        private void RaiseError(string msg)   => ErrorOccurred?.Invoke(msg);
    }
}
