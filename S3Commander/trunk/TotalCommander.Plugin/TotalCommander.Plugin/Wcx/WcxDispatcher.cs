using System;
using System.Diagnostics;

namespace TotalCommander.Plugin.Wcx
{
    static class WcxDispatcher
    {
        private static ITotalCommanderWcxPlugin Plugin
        {
            [DebuggerStepThrough]
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
        }


        public static void SetChangeVolProcW(IntPtr archive, IntPtr callback)
        {
        }

        public static void SetProcessDataProcW(IntPtr archive, IntPtr callback)
        {
        }
        public static void PkSetCryptCallback(IntPtr callback, int number, int flags)
        {
        }


        public static IntPtr OpenArchive(IntPtr archiveData)
        {
            var info = new OpenArchiveInfo(archiveData);
            IntPtr archive;
            info.Result = Plugin.OpenArchive(info.ArchiveName, info.Mode, out archive);
            return archive;
        }

        public static int ReadHeader(IntPtr archive, IntPtr headerData)
        {
            ArchiveHeader header;
            var result = Plugin.ReadHeader(archive, out header);
            if (header != null) header.CopyTo(headerData);
            return (int)result;
        }

        public static int ProcessFile(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            return (int)Plugin.ProcessFile(archive, (ArchiveProcess)operation, Win32.GetString(path), Win32.GetString(name));
        }

        public static int CloseArchive(IntPtr archive)
        {
            return (int)Plugin.CloseArchive(archive);
        }


        public static int PackFiles(IntPtr packedFile, IntPtr subPath, IntPtr srcPath, IntPtr addList, int flags)
        {
            return 0;
        }

        public static int DeleteFiles(IntPtr packedFile, IntPtr deleteList)
        {
            return 0;
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
