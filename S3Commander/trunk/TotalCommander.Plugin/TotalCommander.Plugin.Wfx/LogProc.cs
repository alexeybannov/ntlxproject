using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	delegate void LogProc(
		int pluginNumber,
		int messageType,
		[MarshalAs(UnmanagedType.LPStr)] string logString
	);
}
