using System;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class DefaultParam
	{
		public Int32 Size
		{
			get;
			private set;
		}

		public Int32 PluginInterfaceVersionLow
		{
			get;
			private set;
		}

		public Int32 PluginInterfaceVersionHi
		{
			get;
			private set;
		}

		public string DefaultIniName
		{
			get;
			private set;
		}


		public DefaultParam()
		{

		}

		public static DefaultParam FromPtr(IntPtr ptr)
		{
			var defaultPatam = new DefaultParam();
			if (ptr != IntPtr.Zero)
			{
				var paramStruct = (FsDefaultParamStruct)Marshal.PtrToStructure(ptr, typeof(FsDefaultParamStruct));
				defaultPatam.Size = paramStruct.Size;
				defaultPatam.PluginInterfaceVersionLow = paramStruct.PluginInterfaceVersionLow;
				defaultPatam.PluginInterfaceVersionHi = paramStruct.PluginInterfaceVersionHi;
				defaultPatam.DefaultIniName = paramStruct.DefaultIniName;
			}
			return defaultPatam;
		}
	};
}
