using System.Drawing;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : S3CommanderFile
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
