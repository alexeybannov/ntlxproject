using System.Drawing;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : FileBase
    {
        private Icon icon = Resources.SettingsIcon;


        public override FindData GetFileInfo()
        {
            return new FindData(Resources.Settings);
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return base.Open(window, ref link);
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            if (extractIconFlag == CustomIconFlag.Small)
            {
                icon = this.icon;
                return CustomIconResult.Extracted;
            }
            return CustomIconResult.UseDefault;
        }
    }
}
