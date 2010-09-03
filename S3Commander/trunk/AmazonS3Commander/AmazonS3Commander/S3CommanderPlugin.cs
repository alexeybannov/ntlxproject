using System;
using System.Windows.Forms;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using System.Diagnostics;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class S3CommanderPlugin : TotalCommanderFileSystemPlugin
    {
        public override string PluginName
        {
            get { return "Amazon S3 Commander"; }
        }

        public override BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.Download | BackgroundFlags.Upload | BackgroundFlags.AskUser; }
        }


        protected override IFileSystem CreateFileSystem(FileSystemContext context)
        {
            return new S3CommanderFileSystem(context);
        }

        public override void OnError(Exception error)
        {
            Trace.TraceError(error.ToString());
            MessageBox.Show(error.ToString(), PluginName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
