using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

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
                    return FileOperationResult.ExistsResumeAllowed;
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
                SetProgress(key, localName, offset, info.Size);

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

                        if (SetProgress(key, localName, total += readed, info.Size) == false)
                        {
                            return FileOperationResult.UserAbort;
                        }
                    }
                }

                if (copyFlags.IsSet(CopyFlags.Move))
                {
                    if (!DeleteFile()) return FileOperationResult.WriteError;
                }

                SetProgress(localName, key, 100, 100);
            }
            catch
            {
                return FileOperationResult.ReadError;
            }

            return FileOperationResult.OK;
        }

        public override FileOperationResult Upload(string localName, CopyFlags copyFlags)
        {
            try
            {
                if (copyFlags.IsSet(CopyFlags.ExistsSameCase) && !copyFlags.IsSet(CopyFlags.Overwrite))
                {
                    return FileOperationResult.Exists;
                }
            }
            catch
            {
                return FileOperationResult.ReadError;
            }

            var aborted = false;
            try
            {
                var localFile = new FileInfo(localName);
                if (!localFile.Exists)
                {
                    return FileOperationResult.NotFound;
                }
                var length = localFile.Length;

                SetProgress(localName, key, 0, length);

                Context.S3Service.AddObject(
                    bucket,
                    key,
                    localFile.Length,
                    MimeMapping.GetMimeMapping(localName),
                    stream =>
                    {
                        using (var file = localFile.OpenRead())
                        {
                            var buffer = new byte[1024];
                            var total = 0;
                            while (true)
                            {
                                var readed = file.Read(buffer, 0, buffer.Length);
                                if (readed <= 0) break;

                                stream.Write(buffer, 0, readed);

                                if (SetProgress(localName, key, total += readed, length) == false)
                                {
                                    aborted = true;
                                    throw new OperationCanceledException();
                                }
                            }
                        }
                    }
                );

                if (copyFlags.IsSet(CopyFlags.Move))
                {
                    localFile.Delete();
                }

                SetProgress(localName, key, 100, 100);
            }
            catch
            {
                if (aborted)
                {
                    return FileOperationResult.UserAbort;
                }
                return FileOperationResult.WriteError;
            }

            return FileOperationResult.OK;
        }

        public override bool CreateFolder(string name)
        {
            try
            {
                Context.S3Service.AddObject(
                    bucket,
                    (!string.IsNullOrEmpty(key) ? key + "/" : "") + name + "/",
                    0,
                    null,
                    stream => { });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool DeleteFile()
        {
            try
            {
                Context.S3Service.DeleteObject(bucket, key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool DeleteFolder()
        {
            try
            {
                Context.S3Service.DeleteObject(bucket, key + "/");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
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

        private bool SetProgress(string source, string target, long offset, long length)
        {
            var percent = length != 0 ? (int)(offset * 100 / length) : 100;
            return Context.Progress.SetProgress(source, target, percent);
        }
    }
}
