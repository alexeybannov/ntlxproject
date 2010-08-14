using System.Drawing;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : FileBase
    {
        private readonly Icon icon16x16 = Resources.SettingsIcon;
        private readonly Icon icon32x32 = Resources.SettingsIcon32x32;


        public ConfigurationFile()
        {
            Info = new FindData(Resources.Settings);
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return base.Open(window, ref link);
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            if (extractIconFlag == CustomIconFlag.Small)
            {
                icon = icon16x16;
                return CustomIconResult.Extracted;
            }
            if (extractIconFlag == CustomIconFlag.Large)
            {
                icon = icon32x32;
                return CustomIconResult.Extracted;
            }
            return CustomIconResult.UseDefault;
        }
    }
}
