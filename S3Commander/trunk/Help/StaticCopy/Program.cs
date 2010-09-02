using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ASC.Common.Web;

namespace StaticCopy
{
    internal class UploadFile
    {
        public string FileName { get; set; }
        public string AmazonKey { get; set; }
    }

    class Program
    {
        private static string bucketname;
        private static string acesskey;
        private static string secretkey;
        private static string version;
        private static bool keepCasing;


        static void Main(string[] args)
        {
            List<string> extensions = new List<string>(Settings.Default.extensions.Split('|').Select(x => x.Trim()));
            bucketname = Settings.Default.bucket;
            acesskey = Settings.Default.acesskey;
            secretkey = Settings.Default.secretkey;
            version = Settings.Default.version;
            keepCasing = Settings.Default.keepcasing;
            try
            {
                string srcDir = Settings.Default.dir;
                if (args.Length > 0)
                {
                    srcDir = args[0];
                }
                string[] srcFiles = Directory.GetFiles(Path.Combine(srcDir, Settings.Default.subdir), "*", SearchOption.AllDirectories)
                    .Where((x) => extensions.Contains(Path.GetExtension(x))).ToArray();
                Console.WriteLine("{0} files found.", srcFiles.Length);
                UploadToAmazon(srcFiles, srcDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static ManualResetEvent evt = new ManualResetEvent(false);

        private static List<S3Object> GetS3Objects(AmazonS3 client, string bucket, string prefix)
        {
            var request = new ListObjectsRequest().WithBucketName(bucket).WithPrefix(prefix);
            request.WithMaxKeys(1000);
            var objects = new List<S3Object>();
            ListObjectsResponse response = null;
            do
            {
                response = client.ListObjects(request);
                response.S3Objects.ForEach(entry => objects.Add(entry));
                if (objects.Count == 0) return objects;
                request.Marker = objects[objects.Count - 1].Key;

            } while (response.IsTruncated);
            return objects;
        }

        private static void UploadToAmazon(string[] srcfiles, string srcDir)
        {
            var cfg = new AmazonS3Config() { CommunicationProtocol = Protocol.HTTP, MaxErrorRetry = 3 };
            var client = AWSClientFactory.CreateAmazonS3Client(acesskey, secretkey, cfg);
            //Create bucket if no availible
            Console.WriteLine("Quering bucket");
            var bucket = client.ListBuckets().Buckets.SingleOrDefault((x) => x.BucketName == bucketname);
            if (bucket == null)
            {
                Console.WriteLine("Bucket creating");
                client.PutBucket(new PutBucketRequest().WithBucketName(bucketname));
            }

            //List
            var existingkeys = new Dictionary<string, long>();
            Console.WriteLine("Quering bucket objects");
            var s3objs = GetS3Objects(client, bucketname, version);
            Console.WriteLine("total {0} objects", s3objs.Count);

            foreach (S3Object entry in s3objs)
            {
                existingkeys.Add(entry.Key, entry.Size);
            }




            Queue<UploadFile> filesToUpload = new Queue<UploadFile>(srcfiles.Select(x => new UploadFile() { FileName = x, AmazonKey = GetAmazonKey(x, srcDir) }));

            while (filesToUpload.Count > 0)
            {
                UploadFile file = filesToUpload.Dequeue();
                var finfo = new FileInfo(file.FileName);
                if (existingkeys.ContainsKey(file.AmazonKey) && existingkeys[file.AmazonKey] == finfo.Length)
                {
                    Console.WriteLine("Skipping {0}", file.AmazonKey);
                    continue;//Don't put it
                }
                Console.WriteLine("Sending {0} size {1}", Path.GetFileName(file.AmazonKey),
                                                                   finfo.Length);
                var request = new PutObjectRequest();


                request.WithBucketName(bucketname) //Bucket name
                    .WithKey(file.AmazonKey)
                    .WithCannedACL(S3CannedACL.PublicRead)
                    .WithContentType(
                    MimeMapping.GetMimeMapping(Path.GetFileName(file.FileName)))
                    .WithInputStream(File.OpenRead(file.FileName));

                request.Headers.Add(HttpResponseHeader.CacheControl, string.Format("public, maxage={0}", (int)TimeSpan.FromDays(365).TotalSeconds));
                request.Headers.Add(HttpResponseHeader.ETag, (finfo.Length + finfo.LastWriteTimeUtc.Ticks).ToString());
                request.Headers.Add(HttpResponseHeader.LastModified, DateTime.UtcNow.ToString("R"));
                request.Headers.Add(HttpResponseHeader.Expires, DateTime.UtcNow.Add(TimeSpan.FromDays(365)).ToString("R"));

                try
                {
                    using (client.PutObject(request))
                    {
                        Console.WriteLine("Sent {0} OK", Path.GetFileName(file.AmazonKey));
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Fail: {0}", file.AmazonKey);
                    filesToUpload.Enqueue(file);
                }
            }

        }

        private static string GetAmazonKey(string file, string srcDir)
        {
            var key = version + file.Substring(srcDir.Length).Replace('\\', '/');
            return keepCasing ? key : key.ToLowerInvariant();
        }
    }
}
