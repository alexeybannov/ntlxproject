
namespace LitS3
{
    public class GetAclRequest : S3Request<GetAclResponse>
    {
        public GetAclRequest(S3Service service, string bucketName, string key)
            : base(service, "GET", bucketName, key, "acl")
        {
        }
    }

    public sealed class GetAclResponse : S3Response
    {
        public AccessControlList AccessControlList
        {
            get;
            private set;
        }

        protected override void ProcessResponse()
        {
            AccessControlList = new AccessControlList(Reader);
        }
    }
}
