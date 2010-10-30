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
                UseSsl = false,
                UseSubdomains = true
            };
        }


        public IEnumerable<S3Bucket> GetBuckets()
        {
            return client
                .GetAllBuckets()
                .Select(b => new S3Bucket(b.Name, b.CreationDate));
        }

        public void CreateBucket(string bucketName, S3BucketLocation location)
        {
            client.CreateBucket(bucketName, (location ?? S3BucketLocation.Default).Code);
        }

        public void DeleteBucket(string bucketName)
        {
            client.DeleteBucket(bucketName);
        }


        public IEnumerable<S3Entry> GetObjects(string bucketName, string prefix, string delimiter)
        {
            return client
                .ListAllObjects(bucketName, prefix, delimiter)
                .Select(o =>
                {
                    var f = o as ObjectEntry;
                    return f != null ?
                        (S3Entry)new S3File(f.Name, f.Size, f.LastModified) :
                        (S3Entry)new S3Folder(o.Name);
                });
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

        public bool ObjectExists(string bucketName, string key)
        {
            return client.ObjectExists(bucketName, key);
        }

        public void CopyObject(string sourceBucketName, string sourceKey, string destBucketName, string destKey)
        {
            client.CopyObject(sourceBucketName, sourceKey, destBucketName, destKey);
        }

        public WebHeaderCollection HeadObject(string bucketName, string key)
        {
            return client.HeadObject(bucketName, key);
        }

        public AccessControlList GetObjectAcl(string bucketName, string key)
        {
            return client.GetObjectAcl(bucketName, key);
        }
    }
}
