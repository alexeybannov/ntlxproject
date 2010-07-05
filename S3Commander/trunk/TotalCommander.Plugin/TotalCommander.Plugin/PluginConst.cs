using System;

namespace TotalCommander.Plugin
{
    static class PluginConst
    {
        public const int MAX_PATH = 256;

        public const long NO_FILETIME = unchecked((long)0xFFFFFFFFFFFFFFFE);

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    }
}
