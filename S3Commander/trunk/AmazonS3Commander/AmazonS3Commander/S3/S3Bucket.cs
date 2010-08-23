using System.Collections.Generic;
using System.Drawing;
using LitS3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.S3
{
    class S3Bucket : FileBase
    {
        private readonly S3Service s3Service;

        private readonly string bucketName;


        public S3Bucket(S3Service s3Service, string bucketName)
        {
            this.s3Service = s3Service;
            this.bucketName = bucketName;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return new S3Folder(s3Service, bucketName, string.Empty)
                .GetFiles();
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.Bucket;
            return CustomIconResult.Extracted;
        }
    }
}
