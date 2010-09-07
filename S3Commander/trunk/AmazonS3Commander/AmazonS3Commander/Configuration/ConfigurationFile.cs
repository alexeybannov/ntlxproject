using System.Drawing;
using AmazonS3Commander.Resources;

namespace AmazonS3Commander.Configuration
{
    class ConfigurationFile : S3CommanderFile
    {
        public override Icon GetIcon()
        {
            return Icons.Settings;
        }        
    }
}
