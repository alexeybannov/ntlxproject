using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TotalCommander.Plugin
{
    static class Win32
    {
        public const int MAX_PATH = 260;

        public const int ERROR_NO_MORE_FILES = 18;

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);


        [DllImport("kernel32")]
        public static extern void SetLastError(int errorCode);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static string PtrToStringAnsi(IntPtr ptr)
        {
            return ptr != IntPtr.Zero ? Marshal.PtrToStringAnsi(ptr) : string.Empty;
        }

        public static void WriteStringAnsi(IntPtr ptr, string value)
        {
            WriteStringAnsi(ptr, value, MAX_PATH);
        }

        public static void WriteStringAnsi(IntPtr ptr, string value, int maxLen)
        {
            if (ptr == IntPtr.Zero) return;

            var i = 0;
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(value));
                for (i = 0; i < Math.Min(bytes.Length, maxLen - 1); i++) Marshal.WriteByte(ptr, i, bytes[i]);
            }
            Marshal.WriteByte(ptr, i, 0);//null-terminated
        }
    }
}
