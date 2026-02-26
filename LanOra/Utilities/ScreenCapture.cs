using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace LanOra.Utilities
{
    /// <summary>
    /// Captures the primary screen and returns a JPEG-compressed byte array.
    /// ImageCodecInfo and EncoderParameters are cached for the process lifetime
    /// to avoid repeated GDI+ allocations on every frame.
    /// </summary>
    internal static class ScreenCapture
    {
        // ------------------------------------------------------------------ //
        // Resolution presets                                                  //
        // ------------------------------------------------------------------ //

        public enum Resolution
        {
            Native       = 0,
            HD720p       = 1,   // 1280 × 720
            SD576p       = 2,   // 1024 × 576
            Low450p      = 3    //  800 × 450
        }

        private static readonly int[] PresetWidth  = { 0, 1280, 1024, 800 };
        private static readonly int[] PresetHeight = { 0,  720,  576, 450 };

        // ------------------------------------------------------------------ //
        // Cached codec and parameters (one-time initialisation)              //
        // ------------------------------------------------------------------ //

        private static readonly ImageCodecInfo   _jpegCodec;
        private static readonly EncoderParameters _encParams;

        static ScreenCapture()
        {
            _jpegCodec = FindJpegCodec();

            _encParams = new EncoderParameters(1);
            _encParams.Param[0] = new EncoderParameter(Encoder.Quality, 50L);
        }

        // ------------------------------------------------------------------ //
        // Public API                                                          //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Captures the primary screen at the requested resolution preset.
        /// Returns the JPEG-encoded byte array, or null on failure.
        /// </summary>
        public static byte[] CaptureScreen(Resolution preset = Resolution.HD720p)
        {
            try
            {
                System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;

                using (Bitmap full = new Bitmap(bounds.Width, bounds.Height,
                                               PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(full))
                        g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

                    // Determine target dimensions
                    int targetW, targetH;
                    if (preset == Resolution.Native)
                    {
                        targetW = bounds.Width;
                        targetH = bounds.Height;
                    }
                    else
                    {
                        targetW = PresetWidth [(int)preset];
                        targetH = PresetHeight[(int)preset];
                    }

                    using (Bitmap resized = ResizeBitmap(full, targetW, targetH))
                    // Pre-size to ~25% of raw pixels: a reasonable JPEG estimate
                    using (MemoryStream ms = new MemoryStream(targetW * targetH / 4))
                    {
                        resized.Save(ms, _jpegCodec, _encParams);
                        return ms.ToArray();
                    }
                }
            }
            catch { return null; }
        }

        // ------------------------------------------------------------------ //
        // Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                // InterpolationMode.Low gives the best speed vs quality trade-off
                // for screen-sharing at 5-30 fps over a LAN.
                g.InterpolationMode = InterpolationMode.Low;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }

        private static ImageCodecInfo FindJpegCodec()
        {
            foreach (ImageCodecInfo c in ImageCodecInfo.GetImageEncoders())
                if (c.MimeType == "image/jpeg") return c;
            return null;
        }
    }
}

