using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LANMonitor.Server.Utilities
{
    /// <summary>
    /// Captures the primary screen and compresses it as JPEG for transmission.
    /// </summary>
    internal static class ScreenCapture
    {
        // Target resolution for transmission (reduces bandwidth)
        private const int TargetWidth  = 1280;
        private const int TargetHeight = 720;

        // JPEG quality (0-100). 50 balances quality vs. bandwidth.
        private const long JpegQuality = 50L;

        private static readonly ImageCodecInfo  _jpegCodec;
        private static readonly EncoderParameters _encoderParams;

        static ScreenCapture()
        {
            _jpegCodec     = GetJpegCodec();
            _encoderParams = BuildEncoderParams(JpegQuality);
        }

        /// <summary>
        /// Captures the full primary screen, resizes it, and returns JPEG bytes.
        /// Returns null if capture fails.
        /// </summary>
        public static byte[] CaptureScreen()
        {
            try
            {
                Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                using (Bitmap fullBmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(fullBmp))
                {
                    g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

                    using (Bitmap resized = ResizeBitmap(fullBmp, TargetWidth, TargetHeight))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resized.Save(ms, _jpegCodec, _encoderParams);
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // -------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------

        private static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                // NearestNeighbor is fastest – acceptable for real-time streaming
                g.InterpolationMode  = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.SmoothingMode      = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }

        private static ImageCodecInfo GetJpegCodec()
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.MimeType == "image/jpeg")
                    return codec;
            }
            throw new InvalidOperationException("JPEG codec not found.");
        }

        private static EncoderParameters BuildEncoderParams(long quality)
        {
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            return ep;
        }
    }
}
