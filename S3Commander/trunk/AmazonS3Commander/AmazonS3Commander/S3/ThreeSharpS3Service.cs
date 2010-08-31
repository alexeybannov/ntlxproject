using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Affirma.ThreeSharp;
using Affirma.ThreeSharp.Query;
using Affirma.ThreeSharp.Model;

namespace AmazonS3Commander.S3
{
    class ThreeSharpS3Service : IS3Service
    {
        private IThreeSharp client;

        private readonly string delimiter = "/";


        public ThreeSharpS3Service(string accessKey, string secretKey)
        {
            var config = new ThreeSharpConfig();
            config.AwsAccessKeyID = accessKey;
            config.AwsSecretAccessKey = secretKey;
            config.IsSecure = false;

            client = new ThreeSharpQuery(config);
        }


        public IEnumerable<S3Bucket> GetBuckets()
        {
            using (var request = new BucketListRequest(null))
            using (var response = client.BucketList(request))
            {
                var xml = response.StreamResponseToString();
                return XDocument.Parse(xml)
                    .Root
                    .Element(GetXName("Buckets"))
                    .Elements()
                    .Select(e => new S3Bucket(GetElementValue(e, "Name"), DateTime.Parse(GetElementValue(e, "CreationDate"))));
            }
        }

        public IEnumerable<S3Entry> GetObjects(string bucketName, string prefix)
        {
            using (var request = new BucketListRequest(bucketName, prefix, delimiter))
            using (var response = client.BucketList(request))
            {
                var xml = response.StreamResponseToString();
                return XDocument.Parse(xml)
                    .Root
                    .Elements(GetXName("Contents"))
                    .Select(e =>
                    {
                        return (S3Entry)new S3File(GetElementValue(e, "Key"))
                        {
                            Size = long.Parse(GetElementValue(e, "Size")),
                            CreationDate = DateTime.Parse(GetElementValue(e, "LastModified"))
                        };
                    });
            }
            return null;
        }


        private XName GetXName(string name)
        {
            return XName.Get(name, "http://s3.amazonaws.com/doc/2006-03-01/");
        }

        private string GetElementValue(XElement parent, string name)
        {
            return parent.Element(GetXName(name)).Value;
        }
    }
}
