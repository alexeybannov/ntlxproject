using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wfx.Internal;
using FT = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx
{
	static class WfxExport
	{
		private const int MAX_PATH = 256;

		private static ProgressProc progressProc;
		private static LogProc logProc;
		private static RequestProc requestProc;


		static WfxExport()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
		}


		#region Plugin Installation Name

		[DllExport]
		public static void FsGetDefRootName(IntPtr defRootName, Int32 maxLen)
		{
			if (defRootName == IntPtr.Zero) return;

			var name = WfxFunctions.FsGetDefRootName(maxLen);

			var i = 0;
			if (!string.IsNullOrEmpty(name))
			{
				var bytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(name));
				Array.ForEach(bytes, b => Marshal.WriteByte(defRootName, i++, b));
			}
			Marshal.WriteByte(defRootName, i, 0);//null-terminated
		}

		#endregion

		#region Mandatory (must be implemented)

		[DllExport]
		public static Int32 FsInit(
			int pluginNumber,
			[MarshalAs(UnmanagedType.FunctionPtr)] ProgressProc pProgressProc,
			[MarshalAs(UnmanagedType.FunctionPtr)] LogProc pLogProc,
			[MarshalAs(UnmanagedType.FunctionPtr)] RequestProc pRequestProc)
		{
			progressProc = pProgressProc;
			logProc = pLogProc;
			requestProc = pRequestProc;
			return WfxFunctions.FsInit(pluginNumber, Progress, Log, Request);
		}

		[DllExport]
		public static IntPtr FsFindFirst([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)]string path, IntPtr pFindData)
		{
			return WfxFunctions.FsFindFirst(path, pFindData);
		}

		[DllExport]
		public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
		{
			return WfxFunctions.FsFindNext(handle, pFindData);
		}

		[DllExport]
		public static Int32 FsFindClose(IntPtr handle)
		{
			return WfxFunctions.FsFindClose(handle);
		}

		#endregion

		[DllExport]
		public static void FsSetDefaultParams(IntPtr dps)
		{
			WfxFunctions.FsSetDefaultParams(dps);
		}


		#region File/Dir Operations

		[DllExport]
		public static int FsExecuteFile(
			IntPtr mainWin,
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string verb)
		{
			return WfxFunctions.FsExecuteFile(mainWin, remoteName, verb);
		}

		[DllExport]
		public static int FsRenMovFile(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string oldName,
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string newName,
			bool move,
			bool overWrite,
			IntPtr ri)
		{
			return WfxFunctions.FsRenMovFile(oldName, newName, move, overWrite, ri);
		}

		[DllExport]
		public static bool FsDeleteFile([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName)
		{
			return false;
		}

		[DllExport]
		public static bool FsMkDir([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string path)
		{
			return false;
		}

		[DllExport]
		public static bool FsRemoveDir([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName)
		{
			return false;
		}

		[DllExport]
		public static int FsGetFile(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string localName,
			int copyFlags,
			IntPtr ri)
		{
			return 0;
		}

		[DllExport]
		public static int FsPutFile(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string localName,
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			int copyFlags)
		{
			return 0;
		}

		#endregion

		[DllExport]
		public static bool FsSetAttr([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName, int NewAttr)
		{
			return false;
		}

		[DllExport]
		public static bool FsSetTime(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			[MarshalAs(UnmanagedType.LPStruct)] FT creationTime,
			[MarshalAs(UnmanagedType.LPStruct)] FT lastAccessTime,
			[MarshalAs(UnmanagedType.LPStruct)] FT lastWriteTime)
		{
			return false;
		}

		[DllExport]
		public static bool FsDisconnect([MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string disconnectRoot)
		{
			return false;
		}

		[DllExport]
		public static void FsStatusInfo(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteDir,
			int InfoStartEnd,
			int InfoOperation)
		{

		}

		[DllExport]
		public static int FsExtractCustomIcon(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			int extractFlags,
			IntPtr theIcon)
		{
			return 0;
		}

		[DllExport]
		public static int FsGetPreviewBitmap(
			[MarshalAs(UnmanagedType.LPStr, SizeConst = MAX_PATH)] string remoteName,
			int width,
			int height,
			IntPtr returnedBitmap)
		{
			return 0;
		}

		#region Private Methods

		private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var assemblyFile = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				new AssemblyName(args.Name).Name + ".dll"
			);
			return Assembly.LoadFrom(assemblyFile);
		}


		private static int Progress(int pluginNumber, string sourceName, string targetName, int percentDone)
		{
			return progressProc(pluginNumber, sourceName, targetName, percentDone);
		}

		private static void Log(int pluginNumber, int messageType, string logString)
		{
			logProc(pluginNumber, messageType, logString);
		}

		private static bool Request(int pluginNumber, int requestType, string customTitle, string customText, string defaultText, int maxLen)
		{
			return Request(pluginNumber, requestType, customTitle, customText, defaultText, maxLen);
		}

		#endregion
	}
}
