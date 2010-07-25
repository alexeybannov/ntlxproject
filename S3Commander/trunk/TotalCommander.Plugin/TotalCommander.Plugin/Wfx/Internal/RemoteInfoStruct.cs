using System.Runtime.InteropServices;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct RemoteInfoStruct
	{
		public int SizeLow;
		public int SizeHigh;
		public FileTime LastWrite;
		public int Attr;
	}
}
