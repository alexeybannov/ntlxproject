using System.Drawing;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander
{
    class S3CommanderFile : FileBase
    {
        public S3CommanderContext Context
        {
            get;
            private set;
        }

        protected S3CommanderFile()
        {
        }

        public S3CommanderFile Initialize(S3CommanderContext context)
        {
            Context = context;
            return this;
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = GetIcon();
            return icon != null ? CustomIconResult.Extracted : CustomIconResult.UseDefault;
        }

        protected virtual Icon GetIcon()
        {
            return null;
        }
    }
}
