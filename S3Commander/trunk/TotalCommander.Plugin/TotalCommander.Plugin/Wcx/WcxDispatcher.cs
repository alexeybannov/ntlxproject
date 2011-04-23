using System;
using System.Diagnostics;

namespace TotalCommander.Plugin.Wcx
{
    static class WcxDispatcher
    {
        private static ITotalCommanderWcxPlugin Plugin
        {
            get { return TotalCommanderPluginHolder.GetWcxPlugin(); }
        }


        public static void PackSetDefaultParams(IntPtr dps)
        {
            Plugin.SetDefaultParams(new DefaultParam(dps));
        }

        public static int GetPackerCaps()
        {
            return (int)Plugin.GetPackerCapabilities();
        }

        public static int GetBackgroundFlags()
        {
            return (int)Plugin.GetBackgroundFlags();
        }

        public static void ConfigurePacker(IntPtr window, IntPtr dllInstance)
        {
            Plugin.ConfigurePacker(window, dllInstance);
        }


        public static void SetChangeVolProcW(IntPtr archive, ChangeVolume.Callback callback)
        {
            Plugin.SetChangeVolume(archive, new ChangeVolume(callback));
        }

        public static void SetProcessDataProcW(IntPtr archive, Progress.Callback callback)
        {
            Plugin.SetProgress(archive, new Progress(callback));
        }

        public static void SetCryptCallbackW(Password.Callback callback, int number, int flags)
        {
            Plugin.SetPassword(new Password(callback, number, flags));
        }


        public static IntPtr OpenArchive(IntPtr archiveData)
        {
            var info = new OpenArchiveInfo(archiveData);
            var archive = IntPtr.Zero;
            try
            {
                info.Result = Plugin.OpenArchive(info.ArchiveName, info.Mode, out archive);
            }
            catch (WcxException error)
            {
                info.Result = error.ArchiveResult;
            }
            return archive;
        }

        public static int ReadHeader(IntPtr archive, IntPtr headerData)
        {
            var result = ArchiveResult.Default;
            try
            {
                ArchiveHeader header;
                result = Plugin.ReadHeader(archive, out header);
                if (header != null) header.CopyTo(headerData);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            return (int)result;
        }

        public static int ProcessFile(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.ProcessFile(archive, (ArchiveProcess)operation, Win32.GetString(path), Win32.GetString(name));
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            return (int)result;
        }

        public static int CloseArchive(IntPtr archive)
        {
            var result = ArchiveResult.Default;
            try
            {
                result = Plugin.CloseArchive(archive);
            }
            catch (WcxException error)
            {
                result = error.ArchiveResult;
            }
            return (int)result;
        }


        public static int PackFiles(string packedFile, string subPath, string srcPath, IntPtr addList, int flags)
        {
            return (int)Plugin.PackFiles(packedFile, subPath, srcPath, Win32.GetStringArray(addList), (PackMode)flags);
        }

        public static int DeleteFiles(string packedFile, IntPtr deleteList)
        {
            return (int)Plugin.DeleteFiles(packedFile, Win32.GetStringArray(deleteList));
        }

        public static int StartMemPack(int options, IntPtr fileName)
        {
            return 0;
        }

        public static int PackToMem(int hMemPack, IntPtr bufIn, int inLen, ref Int32 taken, IntPtr bufOut, int outLen, ref Int32 written, int seekBy)
        {
            return 0;
        }

        public static int DoneMemPack(int hMemPack)
        {
            return 0;
        }


        public static bool CanYouHandleThisFile(IntPtr fileName)
        {
            return Plugin.CanYouHandleThisFile(Win32.GetString(fileName));
        }
    }
}
