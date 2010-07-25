using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct FsDefaultParamStruct
	{
		public int Size;
		
		public int VersionLow;
		
		public int VersionHi;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = PluginConst.MAX_PATH)]
		public string DefaultIniName;
	};
}
