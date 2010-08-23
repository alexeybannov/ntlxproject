using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitS3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.S3
{
    class S3Folder : FileBase
    {
        private readonly S3Service s3Service;

        private readonly string bucket;

        private readonly string path;


        public S3Folder(S3Service s3Service, string bucket, string path)
        {
            this.s3Service = s3Service;
            this.bucket = bucket;
            this.path = !string.IsNullOrEmpty(path) && !path.EndsWith("/") ? path + "/" : string.Empty;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return s3Service
                .ListAllObjects(bucket, path)
                .Select(o => ToFindData(o))
                .GetEnumerator();
        }

        private FindData ToFindData(ListEntry entry)
        {
            var file = entry as ObjectEntry;
            if (file != null)
            {
                return new FindData(file.Name, file.Size)
                {
                    LastWriteTime = file.LastModified
                };
            }
            return new FindData(entry.Name, FileAttributes.Directory);
        }
    }
}
