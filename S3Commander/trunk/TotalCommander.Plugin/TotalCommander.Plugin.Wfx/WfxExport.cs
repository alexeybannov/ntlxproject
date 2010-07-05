using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    static class WfxExport
    {
        private const int MAX_PATH = 260;

        private static ProgressProc progressProc;
        private static LogProc logProc;
        private static RequestProc requestProc;


        static WfxExport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }


        [DllExport]
        public static Int32 FsInit(int pluginNumber, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressProc pProgressProc, [MarshalAs(UnmanagedType.FunctionPtr)] LogProc pLogProc, [MarshalAs(UnmanagedType.FunctionPtr)] RequestProc pRequestProc)
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

        [DllExport]
        public static void FsSetDefaultParams(IntPtr dps)
        {
            WfxFunctions.FsSetDefaultParams(dps);
        }

        [DllExport]
        public static void FsGetDefRootName(IntPtr defRootName, Int32 maxLen)
        {
            if (defRootName == IntPtr.Zero) return;
            
            var name = WfxFunctions.FsGetDefRootName(maxLen);

            var i = 0;
            if (!string.IsNullOrEmpty(name))
            {
                var bytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(name));
                for (i = 0; i < bytes.Length; i++) { Marshal.WriteByte(defRootName, i, bytes[i]); }
            }
            Marshal.WriteByte(defRootName, i, 0);//null-terminated
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyFile = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                new AssemblyName(args.Name).Name + ".dll");
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
    }
}
