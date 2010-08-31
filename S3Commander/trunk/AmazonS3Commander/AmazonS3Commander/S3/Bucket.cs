using System.Collections.Generic;
using System.Drawing;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.S3
{
    class Bucket : S3CommanderFile
    {
        private readonly string bucketName;


        public Bucket(string bucketName)
        {
            this.bucketName = bucketName;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return new Entry(bucketName, string.Empty)
                .Initialize(Context)
                .GetFiles();
        }

        protected override Icon GetIcon()
        {
            return Icons.Bucket;
        }
    }
}
