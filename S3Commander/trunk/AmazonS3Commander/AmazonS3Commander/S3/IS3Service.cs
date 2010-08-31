using System.Collections.Generic;
using System.IO;

namespace AmazonS3Commander.S3
{
    interface IS3Service
    {
        IEnumerable<S3Bucket> GetBuckets();

        IEnumerable<S3Entry> GetObjects(string bucketName, string prefix);

        Stream GetObjectStream(string bucketName, string key, long from, out long length);
    }
}
