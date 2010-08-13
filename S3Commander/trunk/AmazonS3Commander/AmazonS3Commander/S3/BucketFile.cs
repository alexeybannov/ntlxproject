using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx.FileSystem;
using LitS3;
using TotalCommander.Plugin.Wfx;
using System.IO;
using AmazonS3Commander.Properties;
using System.Drawing;

namespace AmazonS3Commander.S3
{
    class BucketFile : FileBase
    {
        private readonly Icon icon = Resources.BucketIcon;

        public BucketFile(Bucket bucket)
        {
            Info = new FindData(bucket.Name, FileAttributes.Directory)
            {
                LastWriteTime = bucket.CreationDate
            };
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            if (extractIconFlag == CustomIconFlag.Small)
            {
                icon = this.icon;
                return CustomIconResult.Extracted;
            }
            return CustomIconResult.UseDefault;
        }
    }
}
