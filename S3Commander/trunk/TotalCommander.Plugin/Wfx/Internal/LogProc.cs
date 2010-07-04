using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx.Internal
{
	delegate void LogProc(
		int pluginNumber,
		MessageType messageType,
		[MarshalAs(UnmanagedType.LPStr)] string logString
	);
}
