using System;

namespace TotalCommander.Plugin.Wcx
{
    public class TotalCommanderWcxPlugin : ITotalCommanderWcxPlugin
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
        /// </summary>
        public string PluginIniFile
        {
            get;
            private set;
        }

        public virtual BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.NotSupported; }
        }

        public virtual PackerCapabilities PackerCapabilities
        {
            get { return PackerCapabilities.None; }
        }


        public bool CanYouHandleThisFile(string fileName)
        {
            return false;
        }


        void ITotalCommanderWcxPlugin.SetDefaultParams(DefaultParam dp)
        {
            PluginInterfaceVersion = dp.PluginInterfaceVersion;
            PluginIniFile = dp.DefaultIniFileName;
        }

        BackgroundFlags ITotalCommanderWcxPlugin.GetBackgroundFlags()
        {
            return BackgroundSupport;
        }

        PackerCapabilities ITotalCommanderWcxPlugin.GetPackerCapabilities()
        {
            return PackerCapabilities;
        }
                
        ArchiveResult ITotalCommanderWcxPlugin.OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive)
        {
            archive = IntPtr.Zero;
            return ArchiveResult.Default;
        }

        ArchiveResult ITotalCommanderWcxPlugin.ReadHeader(IntPtr archive, out ArchiveHeader header)
        {
            header = null;
            return ArchiveResult.Default;
        }

        ArchiveResult ITotalCommanderWcxPlugin.ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename)
        {
            return ArchiveResult.Default;
        }

        ArchiveResult ITotalCommanderWcxPlugin.CloseArchive(IntPtr archive)
        {
            return ArchiveResult.Default;
        }
    }
}
