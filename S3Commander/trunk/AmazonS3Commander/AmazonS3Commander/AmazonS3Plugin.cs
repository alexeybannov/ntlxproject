using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class AmazonS3Plugin : TotalCommanderFileSystemPlugin
    {
        public override string PluginName
        {
            get { return "Amazon S3 Commander"; }
        }

        protected override IFileSystem CreateFileSystem()
        {
            return new AmazonS3FileSystem();
        }
    }
}
