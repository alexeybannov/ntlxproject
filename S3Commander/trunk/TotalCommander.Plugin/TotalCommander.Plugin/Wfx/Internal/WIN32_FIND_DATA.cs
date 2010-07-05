using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class WIN32_FIND_DATA
    {
        public int fileAttributes;
        
        public int creationTimeLow;
        public int creationTimeHigh;
        
        public int lastAccessTimeLow;
        public int lastAccessTimeHigh;
        
        public int lastWriteTimeLow;
        public int lastWriteTimeHigh;
        
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
