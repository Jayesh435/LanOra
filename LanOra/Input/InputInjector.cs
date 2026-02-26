using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LanOra.Input
{
    /// <summary>
    /// Injects synthetic mouse and keyboard events into the Windows input
    /// stream using the Win32 <c>SendInput</c> API.
    ///
    /// Requirements:
    ///   – Windows 7 SP1 or later (SendInput present since Windows 2000).
    ///   – <c>mouse_event</c> and <c>keybd_event</c> are intentionally NOT
    ///     used; SendInput is the correct modern API.
    ///   – Absolute mouse coordinates are normalised to 0–65535 using the
    ///     primary screen bounds (matching the screen area captured by
    ///     <see cref="LanOra.Utilities.ScreenCapture"/>).
    ///
    /// Coordinate scaling:
    ///   Viewer sends (relX, relY) within its image display area of size
    ///   (ViewerWidth × ViewerHeight).  The host scales these to primary
    ///   screen pixels and then normalises for SendInput.
    /// </summary>
    internal static class InputInjector
    {
        // ------------------------------------------------------------------ //
        // Win32 structures                                                    //
        // ------------------------------------------------------------------ //

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint      Type;
            public INPUTUNION Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)] public MOUSEINPUT    Mouse;
            [FieldOffset(0)] public KEYBDINPUT    Keyboard;
            [FieldOffset(0)] public HARDWAREINPUT Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int    dx;
            public int    dy;
            public uint   mouseData;
            public uint   dwFlags;
            public uint   time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint   dwFlags;
            public uint   time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint   uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        // INPUT type identifiers
        private const uint INPUT_MOUSE    = 0;
        private const uint INPUT_KEYBOARD = 1;

        // Mouse dwFlags
        private const uint MOUSEEVENTF_MOVE        = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN    = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP      = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN   = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP     = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN  = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP    = 0x0040;
        private const uint MOUSEEVENTF_WHEEL       = 0x0800;
        private const uint MOUSEEVENTF_ABSOLUTE    = 0x8000;

        // Keyboard dwFlags
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP       = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        // ------------------------------------------------------------------ //
        // Public entry point                                                  //
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Dispatches a received <see cref="InputPacket"/>, scaling viewer
        /// coordinates to host primary-screen coordinates before injection.
        /// </summary>
        public static void InjectFromPacket(InputPacket packet)
        {
            int hostX = ScaleX(packet.X, packet.ViewerWidth);
            int hostY = ScaleY(packet.Y, packet.ViewerHeight);

            switch (packet.EventType)
            {
                case InputEventType.MouseMove:
                    InjectMouseMove(hostX, hostY);
                    break;
                case InputEventType.MouseDown:
                    InjectMouseClick(hostX, hostY, packet.Button, true);
                    break;
                case InputEventType.MouseUp:
                    InjectMouseClick(hostX, hostY, packet.Button, false);
                    break;
                case InputEventType.MouseWheel:
                    InjectMouseWheel(packet.Delta);
                    break;
                case InputEventType.KeyDown:
                    InjectKeyDown((Keys)packet.KeyCode);
                    break;
                case InputEventType.KeyUp:
                    InjectKeyUp((Keys)packet.KeyCode);
                    break;
            }
        }

        // ------------------------------------------------------------------ //
        // Mouse injection                                                     //
        // ------------------------------------------------------------------ //

        /// <summary>Moves the cursor to absolute primary-screen coordinates.</summary>
        public static void InjectMouseMove(int hostX, int hostY)
        {
            var input = new INPUT
            {
                Type = INPUT_MOUSE,
                Data = new INPUTUNION
                {
                    Mouse = new MOUSEINPUT
                    {
                        dx          = NormalizeX(hostX),
                        dy          = NormalizeY(hostY),
                        mouseData   = 0,
                        dwFlags     = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
                        time        = 0,
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>Moves the cursor then generates a mouse-button down or up event.</summary>
        public static void InjectMouseClick(int hostX, int hostY, int button, bool isDown)
        {
            uint clickFlag = GetMouseClickFlag(button, isDown);
            if (clickFlag == 0) return;

            var inputs = new[]
            {
                // Move first so the click lands at the correct position
                new INPUT
                {
                    Type = INPUT_MOUSE,
                    Data = new INPUTUNION
                    {
                        Mouse = new MOUSEINPUT
                        {
                            dx          = NormalizeX(hostX),
                            dy          = NormalizeY(hostY),
                            mouseData   = 0,
                            dwFlags     = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
                            time        = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                },
                new INPUT
                {
                    Type = INPUT_MOUSE,
                    Data = new INPUTUNION
                    {
                        Mouse = new MOUSEINPUT
                        {
                            dx          = NormalizeX(hostX),
                            dy          = NormalizeY(hostY),
                            mouseData   = 0,
                            dwFlags     = clickFlag | MOUSEEVENTF_ABSOLUTE,
                            time        = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };
            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>Injects a scroll-wheel event with the given delta.</summary>
        public static void InjectMouseWheel(int delta)
        {
            var input = new INPUT
            {
                Type = INPUT_MOUSE,
                Data = new INPUTUNION
                {
                    Mouse = new MOUSEINPUT
                    {
                        dx          = 0,
                        dy          = 0,
                        mouseData   = (uint)delta,
                        dwFlags     = MOUSEEVENTF_WHEEL,
                        time        = 0,
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        // ------------------------------------------------------------------ //
        // Keyboard injection                                                  //
        // ------------------------------------------------------------------ //

        /// <summary>Injects a virtual key-down event.</summary>
        public static void InjectKeyDown(Keys key) => SendKeyEvent(key, false);

        /// <summary>Injects a virtual key-up event.</summary>
        public static void InjectKeyUp(Keys key) => SendKeyEvent(key, true);

        // ------------------------------------------------------------------ //
        // Private helpers                                                     //
        // ------------------------------------------------------------------ //

        private static void SendKeyEvent(Keys key, bool isUp)
        {
            ushort vk    = (ushort)((int)(key & Keys.KeyCode));
            uint   flags = isUp ? KEYEVENTF_KEYUP : 0u;
            if (IsExtendedKey(key))
                flags |= KEYEVENTF_EXTENDEDKEY;

            var input = new INPUT
            {
                Type = INPUT_KEYBOARD,
                Data = new INPUTUNION
                {
                    Keyboard = new KEYBDINPUT
                    {
                        wVk         = vk,
                        wScan       = 0,
                        dwFlags     = flags,
                        time        = 0,
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        private static bool IsExtendedKey(Keys key)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.RMenu:
                case Keys.RControlKey:
                case Keys.Insert:
                case Keys.Delete:
                case Keys.Home:
                case Keys.End:
                case Keys.Prior:
                case Keys.Next:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.NumLock:
                case Keys.Divide:
                    return true;
                default:
                    return false;
            }
        }

        private static uint GetMouseClickFlag(int button, bool isDown)
        {
            switch ((MouseButtons)button)
            {
                case MouseButtons.Left:   return isDown ? MOUSEEVENTF_LEFTDOWN   : MOUSEEVENTF_LEFTUP;
                case MouseButtons.Right:  return isDown ? MOUSEEVENTF_RIGHTDOWN  : MOUSEEVENTF_RIGHTUP;
                case MouseButtons.Middle: return isDown ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP;
                default:                  return 0;
            }
        }

        /// <summary>
        /// Scales a viewer-relative X coordinate to a primary-screen pixel.
        /// </summary>
        private static int ScaleX(int viewerX, int viewerWidth)
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            if (viewerWidth <= 0) return 0;
            return (int)((double)viewerX / viewerWidth * w);
        }

        /// <summary>
        /// Scales a viewer-relative Y coordinate to a primary-screen pixel.
        /// </summary>
        private static int ScaleY(int viewerY, int viewerHeight)
        {
            int h = Screen.PrimaryScreen.Bounds.Height;
            if (viewerHeight <= 0) return 0;
            return (int)((double)viewerY / viewerHeight * h);
        }

        /// <summary>
        /// Converts a primary-screen pixel X to the 0–65535 range required
        /// by MOUSEEVENTF_ABSOLUTE (without MOUSEEVENTF_VIRTUALDESK).
        /// </summary>
        private static int NormalizeX(int hostX)
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            if (w <= 1) return 0;
            return (int)((double)hostX / (w - 1) * 65535);
        }

        private static int NormalizeY(int hostY)
        {
            int h = Screen.PrimaryScreen.Bounds.Height;
            if (h <= 1) return 0;
            return (int)((double)hostY / (h - 1) * 65535);
        }
    }
}
