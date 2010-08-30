using System;
using System.Collections.Generic;

namespace AmazonS3Commander.S3
{
    interface IS3Service
    {
        IEnumerable<S3Bucket> GetBuckets();

        IEnumerable<S3Entry> GetObjects(string bucketName, string prefix);
    }
}
