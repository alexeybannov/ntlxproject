using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace AmazonS3Commander.S3
{
    class S3Service
    {
        private readonly string accessKey;

        private readonly string secretKey;

        private AmazonS3Client s3client;


        public S3Service(string accessKey, string secretKey)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            var config = new AmazonS3Config()
                .WithCommunicationProtocol(Protocol.HTTP)
                .WithUseSecureStringForAwsSecretKey(false);
            s3client = new AmazonS3Client(accessKey, secretKey, config);
        }


        public List<S3Bucket> GetBuckets()
        {
            return s3client.ListBuckets()
                .Buckets;
        }

        public List<object> GetObjects(string bucketName, string prefix)
        {
            var request = new ListObjectsRequest()
                .WithBucketName(bucketName)
                .WithPrefix(prefix)
                .WithDelimiter("/");
            var response = s3client.ListObjects(request);
            return response
                .S3Objects
                .Select(o => (object)o)
                .Union<object>(response.CommonPrefixes.Select(c => (object)c))
                .ToList();
        }
    }
}
