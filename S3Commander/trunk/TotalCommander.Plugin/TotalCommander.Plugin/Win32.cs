using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin
{
    static class Win32
    {
        public const int MAX_PATH = 260;

        public const int ERROR_NO_MORE_FILES = 18;

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);


        [DllImport("kernel32")]
        public static extern void SetLastError(int errorCode);
    }
}
