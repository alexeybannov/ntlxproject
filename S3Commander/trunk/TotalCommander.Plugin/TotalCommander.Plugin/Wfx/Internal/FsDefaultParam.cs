using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct FsDefaultParam
	{
		public int Size;
		
		public int VersionLow;
		
		public int VersionHigh;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
		public string DefaultIniName;
	};
}
