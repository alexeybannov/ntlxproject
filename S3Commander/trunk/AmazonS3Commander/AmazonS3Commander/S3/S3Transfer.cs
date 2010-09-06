using System;
using System.IO;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.S3
{
    class S3Transfer
    {
        private readonly IS3Service service;

        private readonly string bucketName;

        private readonly string key;


        public S3Transfer(IS3Service service, string bucketName, string key)
        {
            this.service = service;
            this.bucketName = bucketName;
            this.key = key;
        }


        public FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            var offset = 0L;
            try
            {
                var file = new FileInfo(localName);
                if (file.Exists && (copyFlags.Equals(CopyFlags.None) || copyFlags.Equals(CopyFlags.Move)))
                {
                    return FileOperationResult.ExistsResumeAllowed;
                }
                if (copyFlags.IsSet(CopyFlags.Resume))
                {
                    offset = file.Length;
                }
                if (copyFlags.IsSet(CopyFlags.Overwrite))
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                if (Error != null) Error(ex);
                return FileOperationResult.WriteError;
            }

            try
            {
                if (Progress != null) Progress(key, localName, offset, info.Size);

                //download
                using (var stream = service.GetObjectStream(bucketName, key, offset))
                using (var file = new FileStream(localName, FileMode.Append))
                {
                    var buffer = new byte[1024 * 4];
                    var total = offset;
                    while (true)
                    {
                        var readed = stream.Read(buffer, 0, buffer.Length);
                        if (readed <= 0) break;

                        file.Write(buffer, 0, readed);

                        if (Progress != null && Progress(key, localName, total += readed, info.Size) == false)
                        {
                            return FileOperationResult.UserAbort;
                        }
                    }
                }

                if (Progress != null) Progress(key, localName, 100, 100);

                return FileOperationResult.OK;
            }
            catch (Exception ex)
            {
                if (Error != null) Error(ex);
                return FileOperationResult.ReadError;
            }
        }

        public FileOperationResult Upload(string localName, CopyFlags copyFlags)
        {
            if (copyFlags.IsSet(CopyFlags.ExistsSameCase) && !copyFlags.IsSet(CopyFlags.Overwrite))
            {
                return FileOperationResult.Exists;
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

                if (Progress != null) Progress(localName, key, 0, length);

                service.AddObject(
                    bucketName,
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

                                if (Progress != null && Progress(localName, key, total += readed, length) == false)
                                {
                                    aborted = true;
                                    return;
                                }
                            }
                        }
                    }
                );

                if (Progress != null) Progress(localName, key, 100, 100);

                return FileOperationResult.OK;
            }
            catch (Exception ex)
            {
                if (aborted) return FileOperationResult.UserAbort;

                if (Error != null) Error(ex);
                return FileOperationResult.WriteError;
            }
        }


        public event Action<Exception> Error;

        public event Func<string, string, long, long, bool> Progress;
    }
}
