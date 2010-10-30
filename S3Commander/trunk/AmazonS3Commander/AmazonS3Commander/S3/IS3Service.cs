using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using LitS3;

namespace AmazonS3Commander.S3
{
    interface IS3Service
    {
        IEnumerable<S3Bucket> GetBuckets();

        void CreateBucket(string bucketName, S3BucketLocation location);

        void DeleteBucket(string bucketName);


        IEnumerable<S3Entry> GetObjects(string bucketName, string prefix, string delimiter);

        Stream GetObjectStream(string bucketName, string key, long from);

        void AddObject(string bucketName, string key, long bytes, string contentType, Action<Stream> action);

        void DeleteObject(string bucketName, string key);

        bool ObjectExists(string bucketName, string key);

        void CopyObject(string sourceBucketName, string sourceKey, string destBucketName, string destKey);

        WebHeaderCollection HeadObject(string bucketName, string key);

        AccessControlList GetObjectAcl(string bucketName, string key);
    }
}
