using System.Runtime.InteropServices;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct FsFindData
    {
        public int FileAttributes;

        public FileTime CreationTime;
        public FileTime LastAccessTime;
        public FileTime LastWriteTime;

        public int FileSizeHigh;
        public int FileSizeLow;

        public int Reserved0;
        public int Reserved1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
        public string FileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string AlternateFileName;
    }
}
