using System.Runtime.InteropServices;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	struct FsRemoteInfo
	{
		public int SizeLow;
		
        public int SizeHigh;
		
        public FileTime LastWriteTime;
		
        public int Attributes;
	}
}
