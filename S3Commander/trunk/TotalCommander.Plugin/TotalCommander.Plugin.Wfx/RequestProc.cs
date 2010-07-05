using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	delegate bool RequestProc(
		int pluginNumber,
		int requestType,
		[MarshalAs(UnmanagedType.LPStr)] string customTitle,
		[MarshalAs(UnmanagedType.LPStr)] string customText,
		[MarshalAs(UnmanagedType.LPStr)] string defaultText,
		int maxLen
	);
}
