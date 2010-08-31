using System.Collections.Generic;
using System.IO;
using System.Linq;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
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
            var offset = 0L;
            try
            {
                var localFile = new FileInfo(localName);
                if (localFile.Exists && (copyFlags.Equals(CopyFlags.None) || copyFlags.Equals(CopyFlags.Move)))
                {
                    return ResumeAllowed ? FileOperationResult.ExistsResumeAllowed : FileOperationResult.Exists;
                }
                if (copyFlags.IsSet(CopyFlags.Resume))
                {
                    offset = localFile.Length;
                }
                if (copyFlags.IsSet(CopyFlags.Overwrite))
                {
                    localFile.Delete();
                }
            }
            catch
            {
                return FileOperationResult.WriteError;
            }

            try
            {
                SetProgress(localName, offset, info.Size);

                //download
                using (var stream = Context.S3Service.GetObjectStream(bucket, key, offset))
                using (var file = new FileStream(localName, FileMode.Append))
                {
                    var buffer = new byte[1024 * 4];
                    var total = offset;
                    while (true)
                    {
                        var readed = stream.Read(buffer, 0, buffer.Length);
                        if (readed <= 0) break;

                        file.Write(buffer, 0, readed);

                        if (SetProgress(localName, total += readed, info.Size) == false)
                        {
                            return FileOperationResult.UserAbort;
                        }
                    }
                }

                if (copyFlags.IsSet(CopyFlags.Move))
                {
                    if (!Delete()) return FileOperationResult.WriteError;
                }

                SetProgress(localName, 100, 100);
            }
            catch
            {
                return FileOperationResult.ReadError;
            }

            return FileOperationResult.OK;
        }

        public override ExecuteResult Open(TotalCommander.Plugin.TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.YourSelf;
        }

        private FindData ToFindData(S3Entry entry)
        {
            var file = entry as S3File;
            if (file != null)
            {
                return new FindData(file.Name, file.Size)
                {
                    LastWriteTime = file.CreationDate
                };
            }
            return new FindData(entry.Name, FileAttributes.Directory);
        }

        private bool SetProgress(string localName, long offset, long length)
        {
            var percent = length != 0 ? (int)(offset * 100 / length) : 100;
            return Context.Progress.SetProgress(key, localName, percent);
        }
    }
}
