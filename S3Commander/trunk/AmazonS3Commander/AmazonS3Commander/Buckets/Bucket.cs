using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AmazonS3Commander.Files;
using AmazonS3Commander.Resources;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Buckets
{
    class Bucket : S3CommanderFile
    {
        private readonly string bucketName;


        public Bucket(string bucketName)
        {
            if (bucketName == null) throw new ArgumentNullException("bucketName");

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
            using (var form = new NewBucketForm(bucketName))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                if (RS.NewBucket.Equals(form.BucketName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                S3Service.CreateBucket(form.BucketName, form.BucketLocation);
                return true;
            }
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
