using System;
using System.ComponentModel;
using System.Net;

namespace AmazonS3Commander.Files
{
    [Serializable, ImmutableObject(true), DefaultProperty("Name")]
    class EntryInfo
    {
        private DateTime lastModified;

        private long size;


        [DisplayName("Name")]
        [Description("The name of the selected object")]
        public string Name
        {
            get;
            private set;
        }

        [DisplayName("Bucket Name")]
        [Description("The name of the bucket this file belongs to")]
        public string BucketName
        {
            get;
            private set;
        }

        [DisplayName("Object Key")]
        [Description("The path to the selected file inside the bucket")]
        public string Key
        {
            get;
            private set;
        }

        [DisplayName("Last Modified")]
        [Description("The day that the file was last changed")]
        public DateTime LastModified
        {
            get { return lastModified; }
        }

        [DisplayName("Size")]
        [Description("The file of the selected file")]
        public long Size
        {
            get { return size; }
        }

        [DisplayName("Content Type")]
        [Description("The MIME type of the selected file")]
        public string ContentType
        {
            get;
            private set;
        }

        [DisplayName("Etag")]
        [Description("The etag header of the selected file")]
        public string Etag
        {
            get;
            private set;
        }


        public EntryInfo()
        {
            BucketName = "Retrieving data...";
            ContentType = "Retrieving data...";
            Etag = "Retrieving data...";
            Key = "Retrieving data..."; ;
            Name = "Retrieving data...";
        }

        public EntryInfo(string bucketName, string key, WebHeaderCollection headers)
        {
            var index = key.TrimEnd('/').LastIndexOf('/');
            Name = 0 < index ? key.Substring(index + 1) : key;

            BucketName = bucketName;
            Key = key;

            DateTime.TryParse(headers[HttpResponseHeader.LastModified], out lastModified);
            long.TryParse(headers[HttpResponseHeader.ContentLength], out size);
            ContentType = headers[HttpResponseHeader.ContentType];
            Etag = headers[HttpResponseHeader.ETag];
        }
    }
}
