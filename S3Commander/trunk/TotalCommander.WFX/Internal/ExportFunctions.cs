using System;

namespace TotalCommander.WFX.Internal
{
    public static class ExportFunctions
    {
        public static Int32 FsInit(Int32 PluginNr, IntPtr progressProc, IntPtr logProc, IntPtr requestProc)
        {
            return 0;
        }

        public static unsafe IntPtr FsFindFirst(byte* path, IntPtr findData)
        {
            return Const.INVALID_HANDLE_VALUE;
        }

        public static Int32 FsFindNext(IntPtr handle, IntPtr findData)
        {
            return 0;
        }

        public static Int32 FsFindClose(IntPtr handle)
        {
            return 0;
        }


        public static void FsSetDefaultParams(IntPtr dps)
        {

        }

        public static unsafe void FsGetDefRootName(byte* defRootName, Int32 maxLen)
        {

        }
    }
}
