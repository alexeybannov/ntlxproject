using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using TotalCommander.Plugin.Utils;

namespace TotalCommander.Plugin.Wfx.Internal
{
    static class WfxFunctions
    {
        private static ITotalCommanderWfxPlugin Plugin
        {
            get { return TotalCommanderPluginHolder.GetWfxPlugin(); }
        }

        private static IDictionary<IntPtr, IEnumerator> enumerators = new Dictionary<IntPtr, IEnumerator>();


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
            var handle = Win32.INVALID_HANDLE_VALUE;
            try
            {
                IEnumerator enumerator = null;
                var findData = Plugin.FindFirst(path, out enumerator);
                if (findData != null)
                {
                    if (findData == FindData.NoMoreFiles)
                    {
                        Win32.SetLastError(Win32.ERROR_NO_MORE_FILES);
                    }
                    else
                    {
                        findData.CopyTo(pFindData);
                        lock (enumerators)
                        {
                            handle = new IntPtr(enumerators.Count + 1);
                            enumerators[handle] = enumerator;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return handle;
        }

        public static bool FsFindNext(IntPtr handle, IntPtr pFindData)
        {
            try
            {
                IEnumerator enumerator = null;
                lock (enumerators)
                {
                    if (enumerators.ContainsKey(handle)) enumerator = enumerators[handle];
                }
                var findData = Plugin.FindNext(enumerator);
                if (findData != null && findData != FindData.NoMoreFiles)
                {
                    findData.CopyTo(pFindData);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return false;
        }

        public static Int32 FsFindClose(IntPtr handle)
        {
            try
            {
                IEnumerator enumerator = null;
                lock (enumerators)
                {
                    if (enumerators.ContainsKey(handle))
                    {
                        enumerator = enumerators[handle];
                        enumerators.Remove(handle);
                    }
                }
                Plugin.FindClose(enumerator);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return 0;
        }

        public static void FsSetDefaultParams(IntPtr dps)
        {
            try
            {
                Plugin.SetDefaultParams(new DefaultParam(dps));
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        public static string FsGetDefRootName()
        {
            try
            {
                return TotalCommanderPluginHolder.GetWfxPluginName();
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return null;
        }

        public static int FsExecuteFile(IntPtr mainWin, string remoteName, string verb)
        {
            var result = ExecuteResult.Default;
            if (string.IsNullOrEmpty(remoteName)) return (int)result;
            try
            {
                result = Plugin.FileExecute(new TotalCommanderWindow(mainWin), remoteName, verb);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return (int)result;
        }

        public static int FsGetFile(string remoteName, string localName, int copyFlags, IntPtr ri)
        {
            var result = FileOperationResult.Default;
            try
            {
                result = Plugin.FileGet(remoteName, localName, (CopyFlags)copyFlags, new RemoteInfo(ri));
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return (int)result;
        }

        public static int FsPutFile(string localName, string remoteName, int copyFlags)
        {
            var result = FileOperationResult.Default;
            try
            {
                result = Plugin.FilePut(localName, remoteName, (CopyFlags)copyFlags);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return (int)result;
        }

        public static int FsRenMovFile(string oldName, string newName, bool move, bool overWrite, IntPtr ri)
        {
            var result = FileOperationResult.Default;
            try
            {
                result = Plugin.FileRenameMove(oldName, newName, move, overWrite, new RemoteInfo(ri));
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
                result = Plugin.FileRemove(remoteName);
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
                result = Plugin.DirectoryCreate(path);
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
                result = Plugin.DirectoryRemove(remoteName);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return result;
        }

        public static bool FsSetAttr(string remoteName, int newAttr)
        {
            var result = false;
            try
            {
                result = Plugin.SetFileAttributes(remoteName, (FileAttributes)newAttr);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return result;
        }

        public static bool FsSetTime(string remoteName, FILETIME creationTime, FILETIME lastAccessTime, FILETIME lastWriteTime)
        {
            var result = false;
            try
            {
                result = Plugin.SetFileTime(
                    remoteName,
                    DateTimeUtil.FromFileTime(creationTime),
                    DateTimeUtil.FromFileTime(lastAccessTime),
                    DateTimeUtil.FromFileTime(lastWriteTime)
                );
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return result;
        }

        public static bool FsDisconnect(string disconnectRoot)
        {
            var result = false;
            try
            {
                result = Plugin.Disconnect(disconnectRoot);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return result;
        }

        public static void FsStatusInfo(string remoteDir, int infoStartEnd, int infoOperation)
        {
            try
            {
                Plugin.StatusInfo(remoteDir, (StatusInfo)infoStartEnd, (StatusOperation)infoOperation);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        public static int FsExtractCustomIcon(string remoteName, int extractFlags, IntPtr iconHandle)
        {
            var result = CustomIconResult.UseDefault;
            try
            {
                Icon icon = null;
                result = Plugin.GetCustomIcon(remoteName, (CustomIconFlag)extractFlags, out icon);
                if (icon != null) iconHandle = icon.Handle;
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return (int)result;
        }

        public static int FsGetPreviewBitmap(string remoteName, int width, int height, IntPtr bitmapHandle)
        {
            var result = PreviewBitmapResult.None;
            try
            {
                Bitmap bitmap = null;
                result = Plugin.GetPreviewBitmap(remoteName, new Size(width, height), out bitmap);
                if (bitmap != null) bitmapHandle = bitmap.GetHbitmap();
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            return (int)result;
        }


        private static void ProcessError(Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }
}
