﻿using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx
{

	[UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	delegate int ProgressProc(
		int pluginNumber,
		[MarshalAs(UnmanagedType.LPStr)] string sourceName,
		[MarshalAs(UnmanagedType.LPStr)] string targetName,
		int percentDone
	);
}