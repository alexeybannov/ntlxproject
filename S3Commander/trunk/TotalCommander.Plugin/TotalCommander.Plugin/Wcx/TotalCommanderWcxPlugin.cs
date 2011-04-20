using System;

namespace TotalCommander.Plugin.Wcx
{
    public class TotalCommanderWcxPlugin : ITotalCommanderWcxPlugin
    {
        private int counter;
        private string s;


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


        BackgroundFlags ITotalCommanderWcxPlugin.GetBackgroundFlags()
        {
            return BackgroundSupport;
        }

        ArchiveResult ITotalCommanderWcxPlugin.OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive)
        {
            archive = (IntPtr)1;
            s = archiveName;
            return ArchiveResult.Success;
        }

        ArchiveResult ITotalCommanderWcxPlugin.ReadHeader(IntPtr archive, out ArchiveHeader header)
        {
            header = new ArchiveHeader();
            if (counter == 0)
            {
                header = new ArchiveHeader()
                {
                    FileName = "sss",
                    FileAttributes = System.IO.FileAttributes.ReadOnly,
                    FileCRC = 34,
                    FileTime = DateTime.Now,
                    PackedSize = 3444,
                    UnpackedSize = 4 * (long)int.MaxValue,
                    ArchiveName = "ddd"
                };
                counter++;
                return ArchiveResult.Success;
            }
            return ArchiveResult.EndArchive;
        }

        ArchiveResult ITotalCommanderWcxPlugin.ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename)
        {
            return ArchiveResult.Success;
        }

        ArchiveResult ITotalCommanderWcxPlugin.CloseArchive(IntPtr archive)
        {
            counter = 0;
            return ArchiveResult.Success;
        }


        void ITotalCommanderWcxPlugin.SetDefaultParams(DefaultParam dp)
        {
            PluginInterfaceVersion = dp.PluginInterfaceVersion;
            PluginIniFile = dp.DefaultIniFileName;
        }
    }
}
