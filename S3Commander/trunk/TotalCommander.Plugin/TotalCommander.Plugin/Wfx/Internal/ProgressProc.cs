using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	delegate int ProgressProc(
		int pluginNumber,
		[MarshalAs(UnmanagedType.LPStr)] string sourceName,
		[MarshalAs(UnmanagedType.LPStr)] string targetName,
		int percentDone
	);
}
