using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx.FileSystem;
using LitS3;
using TotalCommander.Plugin.Wfx;
using System.IO;
using AmazonS3Commander.Properties;
using System.Drawing;

namespace AmazonS3Commander.S3
{
    class BucketFile : FileBase
    {
        private readonly S3Service s3Service;

        private readonly string bucketName;


        public BucketFile(S3Service s3Service, string bucketName)
        {
            this.s3Service = s3Service;
            this.bucketName = bucketName;
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.Bucket;
            return CustomIconResult.Extracted;
        }
    }
}
