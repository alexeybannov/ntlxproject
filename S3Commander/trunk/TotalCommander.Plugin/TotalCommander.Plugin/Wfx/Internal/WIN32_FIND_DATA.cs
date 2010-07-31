using System.Runtime.InteropServices;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    class WIN32_FIND_DATA
    {
        public int fileAttributes;

        public FileTime creationTime;
        public FileTime lastAccessTime;
        public FileTime lastWriteTime;

        public int nFileSizeHigh;
        public int nFileSizeLow;

        public int dwReserved0;
        public int dwReserved1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PluginConst.MAX_PATH)]
        public string fileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string alternateFileName;
    }
}
