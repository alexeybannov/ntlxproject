using System;
using System.Windows.Forms;
using TotalCommander.Plugin.Utils;

namespace TotalCommander.Plugin.Wfx.Internal
{
	static class WfxFunctions
	{
		private static ITotalCommanderWfxPlugin Plugin
		{
			get { return PluginHolder.GetWfxPlugin(); }
		}

        private static object enumerator;


		public static Int32 FsInit(Int32 pluginNumber, ProgressCallback progress, LogCallback log, RequestCallback request)
		{
			try
			{
				var progresser = new Progress(pluginNumber, progress);
				var logger = new Logger(pluginNumber, log);
				var requestor = new Request(pluginNumber, request);
				Plugin.Init(progresser, logger, requestor);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return 0;
		}

		public static IntPtr FsFindFirst(string path, IntPtr pFindData)
		{
			var findData = new FindData();
			var result = false;
			try
			{
				result = Plugin.FindFirst(path, findData, out enumerator);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			findData.CopyTo(pFindData);
			return result ? IntPtr.Zero : PluginConst.INVALID_HANDLE_VALUE;
		}

		public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
		{
			var findData = new FindData();
			var result = false;
			try
			{
				result = Plugin.FindNext(enumerator, findData);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			findData.CopyTo(pFindData);
			return result;
		}

        public static Int32 FsFindClose(IntPtr handle)
		{
			try
			{
				Plugin.FindClose(enumerator);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			enumerator = null;
			return 0;
		}

		public static void FsSetDefaultParams(IntPtr dps)
		{
			try
			{
				Plugin.SetDefaultParams(DefaultParam.FromPtr(dps));
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
		}

		public static string FsGetDefRootName(Int32 maxLen)
		{
			try
			{
				return StringUtil.First(PluginHolder.GetWfxPluginName(), maxLen - 1);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return null;
		}


		private static void ProcessError(Exception ex)
		{
			MessageBox.Show(ex.ToString());
		}
	}
}
