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


        public static Int32 FsInit(Int32 number, ProgressCallback progress, LogCallback log, RequestCallback request)
		{
			try
			{
				Plugin.Init(
                    new Progress(number, progress),
                    new Logger(number, log),
                    new Request(number, request)
                );
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
		
		public static int FsExecuteFile(IntPtr mainWin, string remoteName, string verb)
		{
			var result = ExecuteResult.Error;
			try
			{
				result = Plugin.ExecuteFile(new MainWindow(mainWin), remoteName, verb);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return (int)result;
		}

		public static int FsRenMovFile(string oldName, string newName, bool move, bool overWrite, IntPtr ri)
		{
			var result = FileOperationResult.NotSupported;
			try
			{
				result = Plugin.RenameMoveFile(oldName, newName, move, overWrite, RemoteInfo.FromPtr(ri));
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return (int)result;
		}

		public static bool FsDeleteFile(string remoteName)
		{
			var result = false;
			try
			{
				result = Plugin.RemoveFile(remoteName);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return result;
		}

		public static bool FsMkDir(string path)
		{
			var result = false;
			try
			{
				result = Plugin.CreateDirectory(path);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return result;
		}

		public static bool FsRemoveDir(string remoteName)
		{
			var result = false;
			try
			{
				result = Plugin.RemoveDirectory(remoteName);
			}
			catch (Exception ex)
			{
				ProcessError(ex);
			}
			return result;
		}
		
		
		private static void ProcessError(Exception ex)
		{
			MessageBox.Show(ex.ToString());
		}
	}
}
