using System.Drawing;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : FileBase
    {
        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return base.Open(window, ref link);
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.Settings;
            return CustomIconResult.Extracted;
        }
    }
}
