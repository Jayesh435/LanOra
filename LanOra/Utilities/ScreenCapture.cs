using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace LanOra.Utilities
{
    /// <summary>
    /// Captures the primary screen and returns a JPEG-compressed byte array
    /// resized to 1280 × 720 at 50 % quality.
    /// </summary>
    internal static class ScreenCapture
    {
        private const int  TargetWidth  = 1280;
        private const int  TargetHeight = 720;
        private const long JpegQuality  = 50L;

        /// <summary>
        /// Captures the primary screen.
        /// Returns the JPEG-encoded byte array, or null on failure.
        /// </summary>
        public static byte[] CaptureScreen()
        {
            try
            {
                System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;

                using (Bitmap full = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(full))
                        g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

                    using (Bitmap   resized = ResizeBitmap(full, TargetWidth, TargetHeight))
                    using (MemoryStream ms  = new MemoryStream())
                    {
                        ImageCodecInfo   codec  = GetJpegCodec();
                        EncoderParameters ep    = new EncoderParameters(1);
                        ep.Param[0] = new EncoderParameter(Encoder.Quality, JpegQuality);

                        resized.Save(ms, codec, ep);
                        return ms.ToArray();
                    }
                }
            }
            catch { return null; }
        }

        private static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }

        private static ImageCodecInfo GetJpegCodec()
        {
            foreach (ImageCodecInfo c in ImageCodecInfo.GetImageEncoders())
                if (c.MimeType == "image/jpeg") return c;
            return null;
        }
    }
}
