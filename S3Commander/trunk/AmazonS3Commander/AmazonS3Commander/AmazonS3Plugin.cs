using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx.FileSystem;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class AmazonS3Plugin : TotalCommanderFileSystemPlugin
    {
        public override string PluginName
        {
            get { return "Amazon S3 Commander"; }
        }

        protected override IFileSystem CreateFileSystem(FileSystemContext context)
        {
            return new AmazonS3FileSystem(context);
        }

        public override BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.AskUser; }
        }

        public override bool TemporaryPanelPlugin
        {
            get { return false; }
        }
    }
}
