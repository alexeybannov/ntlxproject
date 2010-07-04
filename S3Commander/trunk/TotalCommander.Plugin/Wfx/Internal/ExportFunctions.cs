using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TotalCommander.Plugin.Wfx.Internal
{
	static class ExportFunctions
	{
		[DllExport]
		public static Int32 FsInit(Int32 pluginNumber, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressProc progressProc, [MarshalAs(UnmanagedType.FunctionPtr)] LogProc logProc, [MarshalAs(UnmanagedType.FunctionPtr)] RequestProc requestProc)
		{
			return 0;
		}

		[DllExport]
		public static IntPtr FsFindFirst([MarshalAs(UnmanagedType.LPStr, SizeConst = Const.MAX_PATH)]string path, IntPtr pFindData)
		{
			var findData = new FindData();
			object enumerator = null;
			var result = false;
			try
			{
				var plugin = PluginHolder.GetWfxPlugin();
				result = plugin.FindFirst(path, findData, out enumerator);
				findData.CopyTo(pFindData);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return result ? new SafeEnumeratorHandle(enumerator) : SafeEnumeratorHandle.MinusOne;
		}

		[DllExport]
		public static bool FsFindNext(SafeEnumeratorHandle handle, IntPtr pFindData)
		{
			var plugin = PluginHolder.GetWfxPlugin();
			var findData = new FindData();

			var result = plugin.FindNext(handle.Enumerator, findData);

			findData.CopyTo(pFindData);
			return result;
		}

		[DllExport]
		public static Int32 FsFindClose(SafeEnumeratorHandle handle)
		{
			var plugin = PluginHolder.GetWfxPlugin();

			plugin.FindClose(handle.Enumerator);

			handle.Dispose();
			return 0;
		}
		/*
		[DllExport]
		public static void FsSetDefaultParams(IntPtr dps)
		{
			try
			{
				//var plugin = PluginHolder.GetWfxPlugin();
				//plugin.SetDefaultParams(DefaultParam.FromPtr(dps));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}*/
		/*
				[DllExport]
				public static void FsGetDefRootName([Out, MarshalAs(UnmanagedType.LPStr)]string defRootName, Int32 maxLen)
				{
					var plugin = PluginHolder.GetWfxPlugin();
					var name = plugin.Name ?? string.Empty;
					defRootName = name.Length < maxLen ? name : name.Substring(0, maxLen - 1);
				}*/
	}
}
