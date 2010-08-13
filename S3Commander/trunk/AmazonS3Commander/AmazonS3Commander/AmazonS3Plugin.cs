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

        protected override IFileSystem CreateFileSystem()
        {
            return new AmazonS3FileSystem();
        }
        
        public override BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.AskUser; }
        }
    }
}
