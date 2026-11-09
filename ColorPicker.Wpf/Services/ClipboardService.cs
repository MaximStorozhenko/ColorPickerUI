using System.Runtime.InteropServices;
using System.Text;

namespace ColorPickerUI.Services
{
    internal static class ClipboardService
    {
        private const uint CF_UNICODETEXT = 13;
        private const uint GMEM_MOVEABLE = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        public static bool TryCopyText(string text, IntPtr ownerHandle)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text + '\0');

            IntPtr memory = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)bytes.Length);

            if (memory == IntPtr.Zero) return false;

            bool opened = false;
            bool transferred = false;

            try
            {
                IntPtr pointer = GlobalLock(memory);

                if (pointer == IntPtr.Zero) return false;

                try
                {
                    Marshal.Copy(bytes, 0, pointer, bytes.Length);
                }
                finally
                {
                    GlobalUnlock(memory);
                }

                if (!OpenClipboard(ownerHandle)) return false;

                opened = true;

                if (!EmptyClipboard())return false;

                transferred = SetClipboardData(CF_UNICODETEXT, memory) != IntPtr.Zero;

                return transferred;
            }
            finally
            {
                if (opened)
                    CloseClipboard();

                if (!transferred)
                    GlobalFree(memory);
            }
        }
    }
}
