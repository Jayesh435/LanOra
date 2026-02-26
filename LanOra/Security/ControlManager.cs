namespace LanOra.Security
{
    /// <summary>
    /// Tracks whether remote control is currently permitted.
    ///
    /// Control is ONLY active when ALL three conditions are simultaneously true:
    ///   1. The host's "Allow Remote Control" checkbox is checked.
    ///   2. The host explicitly approved the current viewer's request.
    ///   3. A viewer session is currently connected.
    ///
    /// Any single condition becoming false immediately revokes control.
    /// This prevents silent or automatic control activation.
    /// </summary>
    internal sealed class ControlManager
    {
        private volatile bool _allowEnabled;    // condition 1 – host checkbox
        private volatile bool _sessionApproved; // condition 2 – per-session approval
        private volatile bool _connected;       // condition 3 – live session

        /// <summary>
        /// Returns true only when all three conditions are satisfied.
        /// </summary>
        public bool IsControlActive =>
            _allowEnabled && _sessionApproved && _connected;

        // ------------------------------------------------------------------ //
        // State mutators                                                      //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Called when the host's "Allow Remote Control" checkbox changes.
        /// Disabling immediately clears the session approval.
        /// </summary>
        public void SetAllowEnabled(bool allow)
        {
            _allowEnabled = allow;
            if (!allow)
                _sessionApproved = false;
        }

        /// <summary>Called when the host approves a viewer's control request.</summary>
        public void ApproveControl()
        {
            _sessionApproved = true;
        }

        /// <summary>
        /// Revokes the per-session approval.
        /// Called on viewer-initiated release or host-initiated revoke.
        /// </summary>
        public void RevokeControl()
        {
            _sessionApproved = false;
        }

        /// <summary>
        /// Called on viewer connect/disconnect.
        /// Disconnecting also clears the session approval so a new connection
        /// always starts in view-only mode.
        /// </summary>
        public void SetConnected(bool connected)
        {
            _connected = connected;
            if (!connected)
                _sessionApproved = false;
        }
    }
}
