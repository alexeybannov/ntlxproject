using System;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Current plugin interface version and ini file location.
    /// </summary>
    public class DefaultParam
    {
        public Version PluginInterfaceVersion
        {
            get;
            set;
        }

        public string DefaultIniFileName
        {
            get;
            set;
        }


        internal DefaultParam(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var paramStruct = (FsDefaultParamStruct)Marshal.PtrToStructure(ptr, typeof(FsDefaultParamStruct));
                PluginInterfaceVersion = new Version(paramStruct.VersionHi, paramStruct.VersionLow);
                DefaultIniFileName = paramStruct.DefaultIniName;
            }
        }
    };
}
