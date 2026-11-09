using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;
using System.ComponentModel;

namespace ColorPickerUI.Services
{
    internal static class EyedropperService
    {
        #region DLL imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hDc, int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT point);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDect, int xDest, int yDest, int width, int height,
            IntPtr hdcSource, int xSource, int ySource, uint rop);

        [DllImport ("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private const uint MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int VK_ESCAPE = 0x1B;
        #endregion

        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }
        #endregion
        
        public static Color GetScreenPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);

            if(hdc == IntPtr.Zero)
                throw new InvalidOperationException("Unable to get screen device context.");

            try
            {
                uint pixel = GetPixel(hdc, x, y);

                if (pixel == 0xFFFFFFFF)
                    throw new InvalidOperationException("Unable to retrieve pixel color.");

                return Color.FromArgb(
                    255,
                    (byte)(pixel & 0x000000FF),
                    (byte)((pixel & 0x0000FF00) >> 8),
                    (byte)((pixel & 0x00FF0000) >> 16));
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        public static BitmapSource CaptureScreenArea(int x, int y, int width, int height)
        {
            IntPtr screenDC = GetDC(IntPtr.Zero);

            if (screenDC == IntPtr.Zero)
                throw new InvalidOperationException("Unable to get screen DC.");

            IntPtr memoryDC = IntPtr.Zero;
            IntPtr bitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                memoryDC = CreateCompatibleDC(screenDC);
                bitmap = CreateCompatibleBitmap(screenDC, width, height);

                if (memoryDC == IntPtr.Zero || bitmap == IntPtr.Zero)
                    throw new InvalidOperationException("Unable to create bitmap.");

                oldBitmap = SelectObject(memoryDC, bitmap);

                if (!BitBlt(memoryDC, 0, 0, width, height, screenDC, x, y, SRCCOPY))
                    throw new InvalidOperationException("Unable to capture screen area.");

                BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                
                source.Freeze();

                return source;
            }
            finally
            {
                if (oldBitmap != IntPtr.Zero)
                    SelectObject(memoryDC, oldBitmap);

                if (bitmap != IntPtr.Zero)
                    DeleteObject(bitmap);

                if (memoryDC != IntPtr.Zero)
                    DeleteDC(memoryDC);

                ReleaseDC(IntPtr.Zero, screenDC);
            }
        }

        public static (int X, int Y) GetCursorPosition()
        {
            if (!GetCursorPos(out POINT point))
                throw new InvalidOperationException("Unable to get cursor position.");

            return (point.X, point.Y);
        }

        public static BitmapSource CaptureAroundCursor()
        {
            (int x, int y) = GetCursorPosition();

            const int size = 15;
            const int radius = size / 2;

            return CaptureScreenArea(x - radius, y - radius, size, size);
        }

        internal static RECT GetMonitorWorkingArea(int x, int y)
        {
            var point = new POINT
            {
                X = x,
                Y = y
            };

            IntPtr monitor = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);

            MONITORINFO info = new()
            {
                cbSize = Marshal.SizeOf(typeof(MONITORINFO))
            };

            if(!GetMonitorInfo(monitor, ref info))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return info.rcWork;
        }

        public static bool IsEscapePressed()
        {
            return (GetAsyncKeyState(VK_ESCAPE) & 0x8000) != 0;
        }
    }
}
