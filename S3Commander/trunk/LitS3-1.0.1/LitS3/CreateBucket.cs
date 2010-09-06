using System;
using System.IO;
using System.Net;
using System.Text;

namespace LitS3
{
    /// <summary>
    /// Creates a new bucket hosted by S3.
    /// </summary>
    public class CreateBucketRequest : S3Request<CreateBucketResponse>
    {
        private string locationConstraint;

        /// <param name="createInEurope">
        /// True if you want to request that Amazon create this bucket in the Europe location. Otherwise,
        /// false to let Amazon decide.
        /// </param>
        public CreateBucketRequest(S3Service service, string bucketName, string location)
            : base(service, "PUT", bucketName, null, null)
        {
            if (!string.IsNullOrEmpty(location))
            {
                locationConstraint = string.Format("<CreateBucketConfiguration><LocationConstraint>{0}</LocationConstraint></CreateBucketConfiguration>", location);
                WebRequest.ContentLength = locationConstraint.Length;
            }
        }

        void WriteConstraint(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.ASCII);
            writer.Write(locationConstraint);
            writer.Flush();
        }

        public override CreateBucketResponse GetResponse()
        {
            AuthorizeIfNecessary(); // authorize before getting the request stream!

            if (!string.IsNullOrEmpty(locationConstraint))
            {
                using (Stream stream = WebRequest.GetRequestStream())
                    WriteConstraint(stream);
            }

            return base.GetResponse();
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            throw new InvalidOperationException("BeginGetResponse() is not supported for this class yet.");
        }
    }

    /// <summary>
    /// Represents an S3 response for a created bucket.
    /// </summary>
    public class CreateBucketResponse : S3Response
    {
        /// <summary>
        /// The location of the created bucket, as returned by Amazon in the Location header.
        /// </summary>
        public string Location { get; private set; }

        protected override void ProcessResponse()
        {
            Location = WebResponse.Headers[HttpResponseHeader.Location];
        }
    }
}
