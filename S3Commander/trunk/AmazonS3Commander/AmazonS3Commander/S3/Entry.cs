using System.Collections.Generic;
using System.IO;
using System.Linq;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using Amazon.S3.Model;
using System;

namespace AmazonS3Commander.S3
{
    class Entry : S3CommanderFile
    {
        private readonly string bucket;

        private readonly string key;


        public override bool ResumeAllowed
        {
            get { return true; }
        }


        public Entry(string bucket, string key)
        {
            this.bucket = bucket;
            this.key = key;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            return Context.S3Service
                .GetObjects(bucket, !string.IsNullOrEmpty(key) ? key + "/" : "")
                .Select(o => ToFindData(o))
                .GetEnumerator();
        }

        public override FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            /*var localFile = new FileInfo(localName);

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
                Context.S3Service.GetObjectProgress += GetObjectProgress;
                Context.S3Service.GetObject(bucket, key, localName);
            }
            catch
            {
                return FileOperationResult.ReadError;
            }
            finally
            {
                Context.S3Service.GetObjectProgress -= GetObjectProgress;
            }

            if ((copyFlags &= CopyFlags.Move) == CopyFlags.Move)
            {
                if (!Delete()) return FileOperationResult.WriteError;
            }
            */
            return FileOperationResult.OK;
        }

        private FindData ToFindData(object entry)
        {
            var file = entry as S3Object;
            if (file != null)
            {
                return new FindData(file.Key, file.Size)
                {
                    LastWriteTime = DateTime.Parse(file.LastModified)
                };
            }
            return new FindData(((string)entry).Trim('/'), FileAttributes.Directory);
        }

        /*private void GetObjectProgress(object sender, S3ProgressEventArgs e)
        {
            Context.Progress.SetProgress(e.Key, null, e.ProgressPercentage);
        }*/
    }
}
