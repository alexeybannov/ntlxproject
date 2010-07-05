using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct RemoteInfoStruct
	{
		public int SizeLow;
		public int SizeHigh;

		public int LastWriteHigh;
		public int LastWriteLow;

		public int Attr;
	}
}
