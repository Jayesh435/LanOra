using System.Drawing;

namespace LanOra.Theme
{
    /// <summary>
    /// Centralised dark-theme palette and font constants for LanOra.
    /// All forms draw from this single source of truth so the visual
    /// language is consistent across RoleSelectForm, HostForm, and ViewerForm.
    /// </summary>
    internal static class AppTheme
    {
        // ------------------------------------------------------------------ //
        // Colour palette                                                      //
        // ------------------------------------------------------------------ //

        /// <summary>Main window background (#1E1E1E).</summary>
        public static readonly Color Background     = Color.FromArgb(0x1E, 0x1E, 0x1E);

        /// <summary>Panel / card surface (#252526).</summary>
        public static readonly Color Panel          = Color.FromArgb(0x25, 0x25, 0x26);

        /// <summary>Accent blue – primary action colour (#007ACC).</summary>
        public static readonly Color AccentBlue     = Color.FromArgb(0x00, 0x7A, 0xCC);

        /// <summary>Darker accent blue – hover / pressed state.</summary>
        public static readonly Color AccentBlueDark = Color.FromArgb(0x00, 0x5F, 0x9E);

        /// <summary>Success green – "hosting" / "connected" state (#16C60C).</summary>
        public static readonly Color SuccessGreen   = Color.FromArgb(0x16, 0xC6, 0x0C);

        /// <summary>Darker success green – hover state.</summary>
        public static readonly Color SuccessGreenDark = Color.FromArgb(0x10, 0x9A, 0x08);

        /// <summary>Warning / connecting yellow (#FFB900).</summary>
        public static readonly Color WarningYellow  = Color.FromArgb(0xFF, 0xB9, 0x00);

        /// <summary>Error / stop red (#E81123).</summary>
        public static readonly Color ErrorRed       = Color.FromArgb(0xE8, 0x11, 0x23);

        /// <summary>Darker error red – hover state.</summary>
        public static readonly Color ErrorRedDark   = Color.FromArgb(0xB5, 0x0D, 0x1B);

        /// <summary>Primary text – white.</summary>
        public static readonly Color TextPrimary    = Color.White;

        /// <summary>Secondary text – light grey (#CCCCCC).</summary>
        public static readonly Color TextSecondary  = Color.FromArgb(0xCC, 0xCC, 0xCC);

        /// <summary>Custom title-bar background (#1A1A1A).</summary>
        public static readonly Color TitleBar       = Color.FromArgb(0x1A, 0x1A, 0x1A);

        /// <summary>Status-bar background – matches accent blue.</summary>
        public static readonly Color StatusBar      = Color.FromArgb(0x00, 0x7A, 0xCC);

        /// <summary>Subtle card / section border.</summary>
        public static readonly Color CardBorder     = Color.FromArgb(0x3E, 0x3E, 0x42);

        /// <summary>Input box background.</summary>
        public static readonly Color InputBackground = Color.FromArgb(0x2D, 0x2D, 0x30);

        // ------------------------------------------------------------------ //
        // Typography                                                          //
        // ------------------------------------------------------------------ //

        public const string FontFamily = "Segoe UI";

        // ------------------------------------------------------------------ //
        // Branding                                                            //
        // ------------------------------------------------------------------ //

        public const string Developer = "Designed and developed by Jayesh Karemore";

        // ------------------------------------------------------------------ //
        // Helper – create a standard flat-button appearance                  //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Applies flat-style properties to <paramref name="btn"/> so it
        /// matches the dark theme.  The caller supplies the fill colour.
        /// </summary>
        public static void StyleButton(System.Windows.Forms.Button btn, Color backColor)
        {
            btn.BackColor               = backColor;
            btn.ForeColor               = TextPrimary;
            btn.FlatStyle               = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(
                backColor.R + 30 > 255 ? 255 : backColor.R + 30,
                backColor.G + 30 > 255 ? 255 : backColor.G + 30,
                backColor.B + 30 > 255 ? 255 : backColor.B + 30);
            btn.FlatAppearance.BorderSize  = 0;
            btn.Cursor                  = System.Windows.Forms.Cursors.Hand;
        }
    }
}
