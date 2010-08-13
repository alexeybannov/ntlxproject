using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx.FileSystem;
using LitS3;
using TotalCommander.Plugin.Wfx;
using System.IO;

namespace AmazonS3Commander.S3
{
    class BucketFile : FileBase
    {
        public BucketFile(Bucket bucket)
        {
            Info = new FindData(bucket.Name, FileAttributes.Directory)
            {
                LastWriteTime = bucket.CreationDate
            };
        }
    }
}
