using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class WIN32_FIND_DATA
    {
        public int fileAttributes;
        
        // creationTime was an embedded FILETIME structure.
        public int creationTime_lowDateTime;
        public int creationTime_highDateTime;
        
        // lastAccessTime was an embedded FILETIME structure.
        public int lastAccessTime_lowDateTime;
        public int lastAccessTime_highDateTime;
        
        // lastWriteTime was an embedded FILETIME structure.
        public int lastWriteTime_lowDateTime;
        public int lastWriteTime_highDateTime;
        
        public int nFileSizeHigh;
        public int nFileSizeLow;
        
        public int dwReserved0;
        
		public int dwReserved1;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public String fileName;
        
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public String alternateFileName;
    }
}
