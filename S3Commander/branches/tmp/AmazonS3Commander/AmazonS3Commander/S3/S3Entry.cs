using System;

namespace AmazonS3Commander.S3
{
    class S3Entry
    {
        public string Name
        {
            get;
            private set;
        }

        public S3Entry(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var index = key.TrimEnd('/').LastIndexOf('/');
            Name = 0 <= index ? key.Substring(index + 1) : key;
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
