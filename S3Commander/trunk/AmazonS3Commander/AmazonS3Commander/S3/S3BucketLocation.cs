using System;
using System.Collections.Generic;

namespace AmazonS3Commander.S3
{
    class S3BucketLocation
    {
        public static readonly S3BucketLocation USStandard = new S3BucketLocation("", "US Standard");

        public static readonly S3BucketLocation USWest = new S3BucketLocation("us-west-1", "US-West (N. California)");

        public static readonly S3BucketLocation UE = new S3BucketLocation("EU", "UE Ireland");

        public static readonly S3BucketLocation Asia = new S3BucketLocation("ap-southeast-1", "Asia Pacific (Singapore)");

        public static readonly S3BucketLocation Default = USStandard;


        public string Code
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public S3BucketLocation(string code, string name)
        {
            if (code == null) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Code = code;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var loc = obj as S3BucketLocation;
            return loc != null && loc.Code == Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }


        public static IEnumerable<S3BucketLocation> GetAvailableLocations()
        {
            return new[] { USStandard, USWest, UE, Asia };
        }
    }
}
