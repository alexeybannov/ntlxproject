using System;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class DefaultParam
	{
		public Version PluginInterfaceVersion
		{
			get;
			set;
		}

		public string DefaultIniFileName
		{
			get;
			set;
		}


		internal static DefaultParam FromPtr(IntPtr ptr)
		{
			var defaultPatam = new DefaultParam();
			if (ptr != IntPtr.Zero)
			{
				var paramStruct = (FsDefaultParamStruct)Marshal.PtrToStructure(ptr, typeof(FsDefaultParamStruct));
				defaultPatam.PluginInterfaceVersion = new Version(
					paramStruct.PluginInterfaceVersionHi,
					paramStruct.PluginInterfaceVersionLow
				);
				defaultPatam.DefaultIniFileName = paramStruct.DefaultIniName;
			}
			return defaultPatam;
		}
	};
}
