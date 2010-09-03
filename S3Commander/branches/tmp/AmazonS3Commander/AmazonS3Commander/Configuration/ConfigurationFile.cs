using System.Drawing;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : S3CommanderFile
    {
        protected override Icon GetIcon()
        {
            return Icons.Settings;
        }        
    }
}
