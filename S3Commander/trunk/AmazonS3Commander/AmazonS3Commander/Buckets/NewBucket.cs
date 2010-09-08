using System;
using System.Drawing;
using AmazonS3Commander.Resources;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Buckets
{
    class NewBucket : S3CommanderFile
    {
        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            if (window == null) throw new ArgumentNullException("window");

            var bucket = new Bucket(string.Empty).Initialize(Context, S3Service);
            if (bucket.CreateFolder())
            {
                window.Refresh();
            }
            return ExecuteResult.OK;
        }

        public override Icon GetIcon()
        {
            return Icons.NewAccount;
        }
    }
}
