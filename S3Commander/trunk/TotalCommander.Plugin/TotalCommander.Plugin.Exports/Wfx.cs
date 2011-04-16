using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wfx;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TotalCommander.Plugin.Exports
{
    static class Wfx
    {
        private static ProgressProc progressProc;
        private static LogProc logProc;
        private static RequestProc requestProc;
        private static CryptProc cryptProc;


        static Wfx()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }


        #region Plugin Installation Name

        [DllExport]
        public static void FsGetDefRootName(IntPtr defRootName, Int32 maxLen)
        {
            Win32.WriteStringAnsi(defRootName, WfxDispatcher.FsGetDefRootName(), maxLen);
        }

        #endregion

        #region Mandatory (must be implemented)

        [DllExport]
        public static Int32 FsInit(
            Int32 pluginNumber,
            [MarshalAs(UnmanagedType.FunctionPtr)] ProgressProc pProgressProc,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogProc pLogProc,
            [MarshalAs(UnmanagedType.FunctionPtr)] RequestProc pRequestProc)
        {
            progressProc = pProgressProc;
            logProc = pLogProc;
            requestProc = pRequestProc;
            return WfxDispatcher.FsInit(pluginNumber, Progress, Log, Request);
        }

        [DllExport]
        public static IntPtr FsFindFirst([MarshalAs(UnmanagedType.LPStr)]string path, IntPtr pFindData)
        {
            return WfxDispatcher.FsFindFirst(path, pFindData);
        }

        [DllExport]
        public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
        {
            return WfxDispatcher.FsFindNext(handle, pFindData);
        }

        [DllExport]
        public static Int32 FsFindClose(IntPtr handle)
        {
            return WfxDispatcher.FsFindClose(handle);
        }

        #endregion

        [DllExport]
        public static void FsSetDefaultParams(IntPtr dps)
        {
            WfxDispatcher.FsSetDefaultParams(dps);
        }


        #region File/Dir Operations

        [DllExport]
        public static Int32 FsExecuteFile(
            IntPtr mainWin,
            IntPtr remoteName,
            [MarshalAs(UnmanagedType.LPStr)] string verb)
        {
            return WfxDispatcher.FsExecuteFile(mainWin, remoteName, verb);
        }

        [DllExport]
        public static int FsRenMovFile(
            [MarshalAs(UnmanagedType.LPStr)] string oldName,
            [MarshalAs(UnmanagedType.LPStr)] string newName,
            [MarshalAs(UnmanagedType.Bool)] bool move,
            [MarshalAs(UnmanagedType.Bool)] bool overWrite,
            IntPtr ri)
        {
            return WfxDispatcher.FsRenMovFile(oldName, newName, move, overWrite, ri);
        }

        [DllExport]
        public static bool FsDeleteFile([MarshalAs(UnmanagedType.LPStr)] string remoteName)
        {
            return WfxDispatcher.FsDeleteFile(remoteName);
        }

        [DllExport]
        public static bool FsMkDir([MarshalAs(UnmanagedType.LPStr)] string path)
        {
            return WfxDispatcher.FsMkDir(path);
        }

        [DllExport]
        public static bool FsRemoveDir([MarshalAs(UnmanagedType.LPStr)] string remoteName)
        {
            return WfxDispatcher.FsRemoveDir(remoteName);
        }

        [DllExport]
        public static Int32 FsGetFile(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            IntPtr localName,
            Int32 copyFlags,
            IntPtr ri)
        {
            return WfxDispatcher.FsGetFile(remoteName, localName, copyFlags, ri);
        }

        [DllExport]
        public static Int32 FsPutFile(
            [MarshalAs(UnmanagedType.LPStr)] string localName,
            IntPtr remoteName,
            Int32 copyFlags)
        {
            return WfxDispatcher.FsPutFile(localName, remoteName, copyFlags);
        }

        #endregion

        [DllExport]
        public static bool FsSetAttr(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            Int32 newAttr)
        {
            return WfxDispatcher.FsSetAttr(remoteName, newAttr);
        }

        [DllExport]
        public static bool FsSetTime(
            [MarshalAs(UnmanagedType.LPStr)] string remoteName,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime creationTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastAccessTime,
            [MarshalAs(UnmanagedType.LPStruct)] FileTime lastWriteTime)
        {
            return WfxDispatcher.FsSetTime(remoteName, creationTime, lastAccessTime, lastWriteTime);
        }

        [DllExport]
        public static bool FsDisconnect([MarshalAs(UnmanagedType.LPStr)] string disconnectRoot)
        {
            return WfxDispatcher.FsDisconnect(disconnectRoot);
        }

        [DllExport]
        public static void FsStatusInfo(
            [MarshalAs(UnmanagedType.LPStr)] string remoteDir,
            Int32 infoStartEnd,
            Int32 infoOperation)
        {
            WfxDispatcher.FsStatusInfo(remoteDir, infoStartEnd, infoOperation);
        }

        [DllExport]
        public static Int32 FsExtractCustomIcon(
            IntPtr remoteName,
            int extractFlags,
            ref IntPtr theIcon)
        {
            return WfxDispatcher.FsExtractCustomIcon(remoteName, extractFlags, ref theIcon);
        }

        [DllExport]
        public static Int32 FsGetPreviewBitmap(
            IntPtr remoteName,
            Int32 width,
            Int32 height,
            ref IntPtr returnedBitmap)
        {
            return WfxDispatcher.FsGetPreviewBitmap(remoteName, width, height, ref returnedBitmap);
        }

        [DllExport]
        public static void FsSetCryptCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] CryptProc pCryptProc,
            Int32 cryptoNumber,
            Int32 flags)
        {
            cryptProc = pCryptProc;
            WfxDispatcher.FsSetCryptCallback(Crypt, cryptoNumber, flags);
        }

        [DllExport]
        public static bool FsLinksToLocalFiles()
        {
            return WfxDispatcher.FsLinksToLocalFiles();
        }

        [DllExport]
        public static bool FsGetLocalName(IntPtr remoteName, Int32 maxlen)
        {
            return WfxDispatcher.FsGetLocalName(remoteName, maxlen);
        }

        [DllExport]
        public static Int32 FsGetBackgroundFlags()
        {
            return WfxDispatcher.FsGetBackgroundFlags();
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

        private static bool Request(int pluginNumber, int requestType, string customTitle, string customText, StringBuilder defaultText, int maxLen)
        {
            return requestProc(pluginNumber, requestType, customTitle, customText, defaultText, maxLen);
        }

        private static int Crypt(int pluginNumber, int cryptoNumber, int mode, string connectionName, StringBuilder password, int maxLen)
        {
            return cryptProc(pluginNumber, cryptoNumber, mode, connectionName, password, maxLen);
        }

        #endregion


        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate void LogProc(
            Int32 pluginNumber,
            Int32 messageType,
            [MarshalAs(UnmanagedType.LPStr)] string logString
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate int ProgressProc(
            Int32 pluginNumber,
            [MarshalAs(UnmanagedType.LPStr)] string sourceName,
            [MarshalAs(UnmanagedType.LPStr)] string targetName,
            Int32 percentDone
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate bool RequestProc(
            Int32 pluginNumber,
            Int32 requestType,
            [MarshalAs(UnmanagedType.LPStr)] string customTitle,
            [MarshalAs(UnmanagedType.LPStr)] string customText,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder defaultText,
            Int32 maxLen
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate Int32 CryptProc(
            Int32 pluginNumber,
            Int32 cryptoNumber,
            Int32 mode,
            [MarshalAs(UnmanagedType.LPStr)] string connectionName,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder password,
            Int32 maxLen
        );

        #endregion
    }
}
