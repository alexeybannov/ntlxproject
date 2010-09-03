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
            return CreateEntry()
                .GetFiles();
        }

        public override bool CreateFolder(string name)
        {
            return CreateEntry()
                .CreateFolder(name);
        }

        public override Icon GetIcon()
        {
            return Icons.Bucket;
        }

        private S3CommanderFile CreateEntry()
        {
            return new Entry(bucketName, string.Empty)
                .Initialize(Context);
        }
    }
}
