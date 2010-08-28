using System.Collections.Generic;
using System.Drawing;
using LitS3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

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

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.Bucket;
            return CustomIconResult.Extracted;
        }
    }
}
