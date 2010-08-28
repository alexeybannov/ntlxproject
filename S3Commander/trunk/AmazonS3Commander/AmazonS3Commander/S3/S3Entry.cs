using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitS3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.S3
{
    class S3Entry : S3CommanderFile
    {
        private readonly string bucket;

        private readonly string key;


        public override bool ResumeAllowed
        {
            get { return true; }
        }


        public S3Entry(string bucket, string key)
        {
            this.bucket = bucket;
            this.key = key;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return S3CommanderContext.S3Service
                .ListObjects(bucket, !string.IsNullOrEmpty(key) ? key + "/" : string.Empty)
                .Select(o => ToFindData(o))
                .GetEnumerator();
        }

        public override FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            var localFile = new FileInfo(localName);

            if ((copyFlags == CopyFlags.None || copyFlags == CopyFlags.Move) && localFile.Exists)
            {
                return ResumeAllowed ? FileOperationResult.ExistsResumeAllowed : FileOperationResult.Exists;
            }

            var offset = 0L;
            if ((copyFlags &= CopyFlags.Resume) == CopyFlags.Resume)
            {
                offset = localFile.Length;
            }
            if ((copyFlags &= CopyFlags.Overwrite) == CopyFlags.Overwrite)
            {
                try
                {
                    localFile.Delete();
                }
                catch
                {
                    return FileOperationResult.WriteError;
                }
            }

            try
            {
                //download
                S3CommanderContext.S3Service.GetObjectProgress += GetObjectProgress;
                S3CommanderContext.S3Service.GetObject(bucket, key, localName);
            }
            catch
            {
                return FileOperationResult.ReadError;
            }
            finally
            {
                S3CommanderContext.S3Service.GetObjectProgress -= GetObjectProgress;
            }

            if ((copyFlags &= CopyFlags.Move) == CopyFlags.Move)
            {
                if (!Delete()) return FileOperationResult.WriteError;
            }

            return FileOperationResult.OK;
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

        private void GetObjectProgress(object sender, S3ProgressEventArgs e)
        {
            S3CommanderContext.Progress.SetProgress(e.Key, null, e.ProgressPercentage);
        }
    }
}
