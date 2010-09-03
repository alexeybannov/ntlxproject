using System;

namespace AmazonS3Commander.S3
{
    class S3File : S3Entry
    {
        public DateTime LastModified
        {
            get;
            set;
        }

        public long Size
        {
            get;
            set;
        }


        public S3File(string name, long size, DateTime lastModified)
            : base(name)
        {
            Size = size;
            LastModified = lastModified;
        }
    }
}
