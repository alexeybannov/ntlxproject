using System;

namespace AmazonS3Commander.S3
{
    class S3Entry
    {
        public string Key
        {
            get;
            private set;
        }

        public S3Entry(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            Key = key;
        }


        public override string ToString()
        {
            return Key;
        }
    }
}
