using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="DefaultParam"/> is passed to <see cref="ITotalCommanderWfxPlugin.SetDefaultParams"/>
    /// to inform the plugin about the current plugin interface version and ini file location.
    /// </summary>
    /// <seealso cref="ITotalCommanderWfxPlugin.SetDefaultParams"/>
    public class DefaultParam
    {
        /// <summary>
        /// Plugin interface version.
        /// </summary>
        public Version PluginInterfaceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Suggested location+name of the ini file where the plugin could store its data.
        /// This is a fully qualified path+file name, and will be in the same directory as the wincmd.ini.
        /// It's recommended to store the plugin data in this file or at least in this directory,
        /// because the plugin directory or the Windows directory may not be writable!
        /// </summary>
        public string DefaultIniFileName
        {
            get;
            private set;
        }


        internal DefaultParam(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var param = (FsDefaultParam)Marshal.PtrToStructure(ptr, typeof(FsDefaultParam));
                PluginInterfaceVersion = new Version(param.VersionHigh, param.VersionLow / 10);
                DefaultIniFileName = param.DefaultIniName;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct FsDefaultParam
        {
            public int Size;

            public int VersionLow;

            public int VersionHigh;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
            public string DefaultIniName;
        };
    };
}
