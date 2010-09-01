using System.Collections.Generic;
using System.IO;
using System;

namespace AmazonS3Commander.S3
{
    interface IS3Service
    {
        IEnumerable<S3Bucket> GetBuckets();

        IEnumerable<S3Entry> GetObjects(string bucketName, string prefix);

        Stream GetObjectStream(string bucketName, string key, long from);

        void AddObject(string bucketName, string key, long bytes, Action<Stream> action);

        void DeleteObject(string bucketName, string key);
    }
}
