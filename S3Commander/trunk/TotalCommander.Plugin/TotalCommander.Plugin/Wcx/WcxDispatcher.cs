using System;
using System.Collections.Generic;
using System.Text;

namespace TotalCommander.Plugin.Wcx
{
    static class WcxDispatcher
    {
        private static ITotalCommanderWcxPlugin Plugin
        {
            get { return TotalCommanderPluginHolder.GetWcxPlugin(); }
        }


        public static IntPtr OpenArchive(IntPtr archiveData)
        {
            return Plugin.OpenArchive(new OpenArchiveData(archiveData));
        }

        public static int ReadHeader(IntPtr archive, IntPtr headerData)
        {
            return 0;
        }

        public static int ProcessFile(IntPtr archive, int operation, IntPtr path, IntPtr name)
        {
            return 0;
        }

        public static int CloseArchive(IntPtr archive)
        {
            return 0;
        }

        /*public static void SetChangeVolProcW(IntPtr archive, ChangeVolProcW callback)
        {
        }

        public static void SetProcessDataProcW(IntPtr archive, ProcessDataProcW callback)
        {
        }*/


        public static int PackFiles(IntPtr packedFile, IntPtr subPath, IntPtr srcPath, IntPtr addList, int flags)
        {
            return 0;
        }

        public static int DeleteFiles(IntPtr packedFile, IntPtr deleteList)
        {
            return 0;
        }

        public static int GetPackerCaps()
        {
            return 0;
        }

        public static void ConfigurePacker(IntPtr window, IntPtr dllInstance)
        {
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
            return false;
        }

        public static void PackSetDefaultParams(IntPtr dps)
        {
        }

/*
        public static void PkSetCryptCallback(CryptProcW callback, int number, int flags)
        {
        }
*/

        public static int GetBackgroundFlags()
        {
            return 0;
        }
    }
}
