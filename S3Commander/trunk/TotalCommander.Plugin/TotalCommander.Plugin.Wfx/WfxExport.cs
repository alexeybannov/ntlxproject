using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wfx.Internal;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Wfx
{
    static class WfxExport
    {
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

            var name = WfxFunctions.FsGetDefRootName();

            var i = 0;
            if (!string.IsNullOrEmpty(name))
            {
                var bytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(name));
                for (i = 0; i < Math.Min(bytes.Length, maxLen - 1); i++) Marshal.WriteByte(defRootName, i, bytes[i]);
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
        public static IntPtr FsFindFirst([MarshalAs(UnmanagedType.LPStr)]string path, IntPtr pFindData)
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
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            [MarshalAs(UnmanagedType.LPStr)] string verb)
        {
            return WfxFunctions.FsExecuteFile(mainWin, remoteName, verb);
        }

        [DllExport]
        public static int FsRenMovFile(
            [MarshalAs(UnmanagedType.LPStr)] string oldName,
            [MarshalAs(UnmanagedType.LPStr)] string newName,
            bool move,
            bool overWrite,
            IntPtr ri)
        {
            return WfxFunctions.FsRenMovFile(oldName, newName, move, overWrite, ri);
        }

        [DllExport]
        public static bool FsDeleteFile([MarshalAs(UnmanagedType.LPStr)] string remoteName)
        {
            return WfxFunctions.FsDeleteFile(remoteName);
        }

        [DllExport]
        public static bool FsMkDir([MarshalAs(UnmanagedType.LPStr)] string path)
        {
            return WfxFunctions.FsMkDir(path);
        }

        [DllExport]
        public static bool FsRemoveDir([MarshalAs(UnmanagedType.LPStr)] string remoteName)
        {
            return WfxFunctions.FsRemoveDir(remoteName);
        }

        [DllExport]
        public static int FsGetFile(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            [MarshalAs(UnmanagedType.LPStr)] string localName,
            int copyFlags,
            IntPtr ri)
        {
            return WfxFunctions.FsGetFile(remoteName, localName, copyFlags, ri);
        }

        [DllExport]
        public static int FsPutFile(
            [MarshalAs(UnmanagedType.LPStr)] string localName,
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            int copyFlags)
        {
            return WfxFunctions.FsPutFile(remoteName, localName, copyFlags);
        }

        #endregion

        [DllExport]
        public static bool FsSetAttr(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            int newAttr)
        {
            return WfxFunctions.FsSetAttr(remoteName, newAttr);
        }

        [DllExport]
        public static bool FsSetTime(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime creationTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastAccessTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastWriteTime)
        {
            return WfxFunctions.FsSetTime(remoteName, creationTime, lastAccessTime, lastWriteTime);
        }

        [DllExport]
        public static bool FsDisconnect([MarshalAs(UnmanagedType.LPStr)] string disconnectRoot)
        {
            return WfxFunctions.FsDisconnect(disconnectRoot);
        }

        [DllExport]
        public static void FsStatusInfo(
            [MarshalAs(UnmanagedType.LPStr)] string remoteDir,
            int infoStartEnd,
            int infoOperation)
        {
            WfxFunctions.FsStatusInfo(remoteDir, infoStartEnd, infoOperation);
        }

        [DllExport]
        public static int FsExtractCustomIcon(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            int extractFlags,
            IntPtr theIcon)
        {
            return WfxFunctions.FsExtractCustomIcon(remoteName, extractFlags, theIcon);
        }

        [DllExport]
        public static int FsGetPreviewBitmap(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            int width,
            int height,
            IntPtr returnedBitmap)
        {
            return WfxFunctions.FsGetPreviewBitmap(remoteName, width, height, returnedBitmap);
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
            return requestProc(pluginNumber, requestType, customTitle, customText, defaultText, maxLen);
        }

        #endregion


        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate void LogProc(
            int pluginNumber,
            int messageType,
            [MarshalAs(UnmanagedType.LPStr)] string logString
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate int ProgressProc(
            int pluginNumber,
            [MarshalAs(UnmanagedType.LPStr)] string sourceName,
            [MarshalAs(UnmanagedType.LPStr)] string targetName,
            int percentDone
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate bool RequestProc(
            int pluginNumber,
            int requestType,
            [MarshalAs(UnmanagedType.LPStr)] string customTitle,
            [MarshalAs(UnmanagedType.LPStr)] string customText,
            [MarshalAs(UnmanagedType.LPStr)] string defaultText,
            int maxLen
        );

        #endregion
    }
}
