using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander
{
    class S3CommanderFile : FileBase
    {
        public S3CommanderContext S3CommanderContext
        {
            get;
            private set;
        }

        protected S3CommanderFile()
        {
        }

        public S3CommanderFile Initialize(S3CommanderContext context)
        {
            S3CommanderContext = context;
            return this;
        }
    }
}
