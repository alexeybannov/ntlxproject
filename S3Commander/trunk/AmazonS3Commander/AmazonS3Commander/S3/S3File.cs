using System;

namespace AmazonS3Commander.S3
{
    class S3File : S3Entry
    {
        public DateTime CreationDate
        {
            get;
            set;
        }

        public long Size
        {
            get;
            set;
        }


        public S3File(string name)
            : base(name)
        {

        }
    }
}
