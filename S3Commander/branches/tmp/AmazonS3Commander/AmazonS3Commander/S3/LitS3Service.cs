using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using LitS3;

namespace AmazonS3Commander.S3
{
    class LitS3Service : IS3Service
    {
        private readonly S3Service client;


        public LitS3Service(string accessKey, string secretKey)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            client = new S3Service()
            {
                AccessKeyID = accessKey,
                SecretAccessKey = secretKey,
                UseSsl = false
            };
        }


        public IEnumerable<S3Bucket> GetBuckets()
        {
            return client
                .GetAllBuckets()
                .Select(b => new S3Bucket(b.Name, b.CreationDate));
        }

        public IEnumerable<S3Entry> GetObjects(string bucketName, string prefix)
        {
            return client
                .ListAllObjects(bucketName, prefix)
                .Select(o =>
                {
                    var f = o as ObjectEntry;
                    return f != null ?
                        (S3Entry)new S3File(f.Name, f.Size, f.LastModified) :
                        (S3Entry)new S3Folder(o.Name);
                })
                .Where(e => !string.IsNullOrEmpty(e.Key));
        }

        public Stream GetObjectStream(string bucketName, string key, long from)
        {
            return client.GetObjectStream(bucketName, key, from);
        }

        public void AddObject(string bucketName, string key, long bytes, string contentType, Action<Stream> action)
        {
            client.AddObject(bucketName, key, bytes, contentType, default(CannedAcl), action);
        }

        public void DeleteObject(string bucketName, string key)
        {
            client.DeleteObject(bucketName, key);
        }

        public void CopyObject(string sourceBucketName, string sourceKey, string destBucketName, string destKey)
        {
            client.CopyObject(sourceBucketName, sourceKey, destBucketName, destKey);
        }
    }
}
