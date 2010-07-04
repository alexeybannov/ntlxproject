using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct FsDefaultParamStruct
	{
		public Int32 Size;
		
		public Int32 PluginInterfaceVersionLow;
		
		public Int32 PluginInterfaceVersionHi;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.MAX_PATH)]
		public string DefaultIniName;
	};
}
