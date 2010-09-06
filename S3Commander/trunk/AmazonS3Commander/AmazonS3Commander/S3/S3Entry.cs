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

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var entry = obj as S3Entry;
            return entry != null && entry.Key == Key;
        }
    }
}
