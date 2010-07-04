﻿using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Ntlx.MSBuild.Tasks
{
	static class SystemPathProvider
	{
		private static string sdkLocation;


		public static string FrameworkVersion
		{
			get { return RuntimeEnvironment.GetSystemVersion().Substring(0, 4); }
		}

		public static string FrameworkPath
		{
			get { return RuntimeEnvironment.GetRuntimeDirectory(); }
		}

		public static string MicrosoftSdkPath
		{
			get
			{
				if (sdkLocation == null)
				{
					var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows");
					if (key != null)
					{
						var location = key.GetValue("CurrentInstallFolder");
						if (location != null) sdkLocation = Path.Combine(location.ToString(), "Bin");
					}
				}
				return sdkLocation;
			}
		}
	}
}
