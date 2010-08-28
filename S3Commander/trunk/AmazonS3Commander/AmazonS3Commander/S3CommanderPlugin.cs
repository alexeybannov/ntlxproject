using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx.FileSystem;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class S3CommanderPlugin : TotalCommanderFileSystemPlugin
    {
        public override string PluginName
        {
            get { return "Amazon S3 Commander"; }
        }

        protected override IFileSystem CreateFileSystem(FileSystemContext context)
        {
            return new S3CommanderFileSystem(context);
        }

        public override BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.Download | BackgroundFlags.Upload; }
        }

        public override bool TemporaryPanelPlugin
        {
            get { return false; }
        }
    }
}
