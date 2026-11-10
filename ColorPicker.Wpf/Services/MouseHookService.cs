using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ColorPickerUI.Services
{
    internal class MouseHookService : IDisposable
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        private readonly LowLevelMouseProc hookCallback;
        private IntPtr hookId = IntPtr.Zero;

        public event Action? LeftButtonDown;

        public MouseHookService()
        {
            hookCallback = HookCallback;
        }

        public void Start()
        {
            if (hookId != IntPtr.Zero) return;

            using Process process = Process.GetCurrentProcess();
            using ProcessModule? module = process.MainModule;

            hookId = SetWindowsHookEx(
                WH_MOUSE_LL, hookCallback, GetModuleHandle(module?.ModuleName), 0);

            if (hookId == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                try
                {
                    LeftButtonDown?.Invoke();
                }
                catch
                {
                    // We do not allow exceptions to the exceptions for system callback restrictions.
                }

                return (IntPtr)1;
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public void Stop()
        {
            if (hookId == IntPtr.Zero) return;

            if(UnhookWindowsHookEx(hookId))
                hookId = IntPtr.Zero;
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
