using System;
using System.Diagnostics;
using System.Threading;

namespace LanOra.Monitoring
{
    /// <summary>
    /// Thread-safe tracker for real-time streaming performance metrics.
    /// Measures frames-per-second and bytes-per-second over a rolling
    /// one-second window using only Stopwatch (no Task or async).
    /// </summary>
    internal sealed class PerformanceTracker
    {
        // ------------------------------------------------------------------ //
        // Private state                                                       //
        // ------------------------------------------------------------------ //

        private readonly Stopwatch _sw        = Stopwatch.StartNew();
        private readonly object    _lock      = new object();

        // Counters accumulated during the current window
        private int  _frameCount;
        private long _byteCount;

        // Last published snapshot
        private double _currentFps;
        private double _currentBytesPerSec;

        // Tick of the last window reset
        private long   _windowStartTicks;

        // ------------------------------------------------------------------ //
        // Public read-only properties (snapshot, thread-safe)               //
        // ------------------------------------------------------------------ //

        /// <summary>Frames per second – updated every ~1 second.</summary>
        public double Fps
        {
            get { lock (_lock) return _currentFps; }
        }

        /// <summary>Bytes per second – updated every ~1 second.</summary>
        public double BytesPerSecond
        {
            get { lock (_lock) return _currentBytesPerSec; }
        }

        /// <summary>Kilobytes per second helper.</summary>
        public double KbPerSecond => BytesPerSecond / 1024.0;

        // ------------------------------------------------------------------ //
        // Update methods (call from any thread)                              //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Records one transmitted/received frame of <paramref name="byteCount"/>
        /// bytes. Call this immediately after each frame is sent or received.
        /// </summary>
        public void RecordFrame(int byteCount)
        {
            lock (_lock)
            {
                _frameCount++;
                _byteCount += byteCount;
                TryFlushWindow();
            }
        }

        /// <summary>Resets all counters (e.g., on disconnect).</summary>
        public void Reset()
        {
            lock (_lock)
            {
                _frameCount           = 0;
                _byteCount            = 0;
                _currentFps           = 0.0;
                _currentBytesPerSec   = 0.0;
                _windowStartTicks     = _sw.ElapsedTicks;
            }
        }

        // ------------------------------------------------------------------ //
        // Private helpers                                                    //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Publishes accumulated counters whenever the elapsed window exceeds
        /// one second, then resets them for the next window.
        /// Must be called inside the lock.
        /// </summary>
        private void TryFlushWindow()
        {
            long elapsed = _sw.ElapsedTicks - _windowStartTicks;
            double seconds = (double)elapsed / Stopwatch.Frequency;

            if (seconds < 1.0) return;

            // Publish snapshot
            _currentFps         = _frameCount / seconds;
            _currentBytesPerSec = _byteCount  / seconds;

            // Reset for next window
            _frameCount       = 0;
            _byteCount        = 0;
            _windowStartTicks = _sw.ElapsedTicks;
        }
    }
}
