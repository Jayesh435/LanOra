using System.IO;

namespace LanOra.Input
{
    /// <summary>Mouse or keyboard event type carried in an <see cref="InputPacket"/>.</summary>
    internal enum InputEventType : byte
    {
        MouseMove  = 1,
        MouseDown  = 2,
        MouseUp    = 3,
        MouseWheel = 4,
        KeyDown    = 5,
        KeyUp      = 6
    }

    /// <summary>
    /// A single mouse or keyboard input event captured on the viewer side and
    /// transmitted to the host for injection via <c>SendInput</c>.
    ///
    /// Serialized size: 29 bytes (1 + 7 × 4).
    ///
    /// Coordinate scaling: the viewer includes its display area dimensions
    /// (<see cref="ViewerWidth"/> / <see cref="ViewerHeight"/>) so the host
    /// can map viewer-relative coordinates to host screen coordinates:
    ///   hostX = (X / ViewerWidth)  * hostScreenWidth
    ///   hostY = (Y / ViewerHeight) * hostScreenHeight
    /// </summary>
    internal sealed class InputPacket
    {
        /// <summary>Event category.</summary>
        public InputEventType EventType    { get; set; }

        /// <summary>X coordinate within the viewer's image display area.</summary>
        public int            X           { get; set; }

        /// <summary>Y coordinate within the viewer's image display area.</summary>
        public int            Y           { get; set; }

        /// <summary>
        /// <see cref="System.Windows.Forms.MouseButtons"/> value for mouse
        /// events; 0 for keyboard events.
        /// </summary>
        public int            Button      { get; set; }

        /// <summary>Scroll delta for <see cref="InputEventType.MouseWheel"/>.</summary>
        public int            Delta       { get; set; }

        /// <summary><see cref="System.Windows.Forms.Keys"/> value for keyboard events.</summary>
        public int            KeyCode     { get; set; }

        /// <summary>Width of the viewer's image display area in pixels.</summary>
        public int            ViewerWidth  { get; set; }

        /// <summary>Height of the viewer's image display area in pixels.</summary>
        public int            ViewerHeight { get; set; }

        // ------------------------------------------------------------------ //
        // Serialization                                                       //
        // ------------------------------------------------------------------ //

        private const int SerializedSize = 29; // 1 + 7*4

        /// <summary>Serializes this packet to a 29-byte array.</summary>
        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream(SerializedSize))
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((byte)EventType);
                bw.Write(X);
                bw.Write(Y);
                bw.Write(Button);
                bw.Write(Delta);
                bw.Write(KeyCode);
                bw.Write(ViewerWidth);
                bw.Write(ViewerHeight);
                bw.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>Deserializes a packet from a byte array produced by <see cref="Serialize"/>.</summary>
        public static InputPacket Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                return new InputPacket
                {
                    EventType    = (InputEventType)br.ReadByte(),
                    X            = br.ReadInt32(),
                    Y            = br.ReadInt32(),
                    Button       = br.ReadInt32(),
                    Delta        = br.ReadInt32(),
                    KeyCode      = br.ReadInt32(),
                    ViewerWidth  = br.ReadInt32(),
                    ViewerHeight = br.ReadInt32()
                };
            }
        }
    }
}
