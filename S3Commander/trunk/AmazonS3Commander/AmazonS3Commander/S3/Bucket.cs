using System;
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
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException("bucketName");

            this.bucketName = bucketName;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation == StatusOperation.List)
            {
                return new Entry(bucketName, string.Empty)
                    .Initialize(Context, S3Service)
                    .GetFiles();
            }
            return EmptyFindDataEnumerator;
        }

        public override bool CreateFolder()
        {
            S3Service.CreateBucket(bucketName, null);
            return true;
        }

        public override bool DeleteFolder()
        {
            S3Service.DeleteBucket(bucketName);
            return true;
        }

        public override Icon GetIcon()
        {
            return Icons.Bucket;
        }
    }
}
