namespace LanOra.Networking
{
    /// <summary>
    /// Packet type identifiers for the LanOra TCP protocol.
    ///
    /// Wire format after PIN authentication:
    ///   [1 byte PacketType][4 bytes PayloadLength (little-endian)][Payload bytes]
    ///
    /// Directionality:
    ///   Frame           – Server → Viewer
    ///   ControlRequest  – Viewer → Server
    ///   ControlResponse – Server → Viewer
    ///   MouseEvent      – Viewer → Server
    ///   KeyboardEvent   – Viewer → Server
    ///   ControlRelease  – Bidirectional (either side can terminate control)
    /// </summary>
    internal enum PacketType : byte
    {
        Frame           = 1,
        ControlRequest  = 2,
        ControlResponse = 3,
        MouseEvent      = 4,
        KeyboardEvent   = 5,
        ControlRelease  = 6
    }
}
