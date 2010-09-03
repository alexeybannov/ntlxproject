using System;

namespace AmazonS3Commander.S3
{
    class S3Bucket : S3Entry
    {
        public DateTime CreationDate
        {
            get;
            set;
        }


        public S3Bucket(string name, DateTime creationDate)
            : base(name)
        {
            CreationDate = creationDate;
        }
    }
}
