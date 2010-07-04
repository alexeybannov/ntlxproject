using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	delegate bool RequestProc(
		int pluginNumber,
		RequestType requestType,
		[MarshalAs(UnmanagedType.LPStr)] string customTitle,
		[MarshalAs(UnmanagedType.LPStr)] string customText,
		[MarshalAs(UnmanagedType.LPStr)] string defaultText,
		int maxLen
	);
}
