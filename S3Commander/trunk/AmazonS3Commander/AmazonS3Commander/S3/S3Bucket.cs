using System.Collections.Generic;
using System.Drawing;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.S3
{
    class S3Bucket : S3CommanderFile
    {
        private readonly string bucketName;


        public S3Bucket(string bucketName)
        {
            this.bucketName = bucketName;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return new S3Entry(bucketName, string.Empty)
                .Initialize(S3CommanderContext)
                .GetFiles();
        }

        protected override Icon GetIcon()
        {
            return Icons.Bucket;
        }
    }
}
