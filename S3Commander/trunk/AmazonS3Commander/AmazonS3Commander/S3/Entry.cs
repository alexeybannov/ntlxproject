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
        private readonly string bucketName;

        private readonly string key;

        private string FolderKey
        {
            get { return key != string.Empty ? key + "/" : key; }
        }


        public Entry(string bucketName, string key)
        {
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException("bucketName");
            if (key == null) throw new ArgumentNullException("key");

            this.bucketName = bucketName;
            this.key = key;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation == StatusOperation.CalculateSize ||
                Context.CurrentOperation == StatusOperation.Delete ||
                Context.CurrentOperation == StatusOperation.RenameMoveMulti)
            {
                return S3Service
                    .GetObjects(bucketName, FolderKey, "")
                    .Where(o => o is S3Entry)
                    .Select(o => ToFindData(o))
                    .GetEnumerator();
            }
            return S3Service
                .GetObjects(bucketName, FolderKey, "/")
                .Where(o => !string.IsNullOrEmpty(o.Key))
                .Select(o => ToFindData(o))
                .GetEnumerator();
        }

        public override FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            return GetTransfer().Download(localName, copyFlags, info);
        }

        public override FileOperationResult Upload(string localName, CopyFlags copyFlags)
        {
            return GetTransfer().Upload(localName, copyFlags);
        }

        public override bool DeleteFile()
        {
            S3Service.DeleteObject(bucketName, key);
            return true;
        }

        public override FileOperationResult CopyTo(S3CommanderFile dest, bool overwrite, bool move, RemoteInfo info)
        {
            var entry = dest as Entry;
            if (entry == null) return FileOperationResult.NotSupported;

            try
            {
                if (!overwrite && S3Service.ObjectExists(entry.bucketName, entry.key))
                {
                    return FileOperationResult.Exists;
                }

                var source = bucketName + "/" + key;
                var target = entry.bucketName + "/" + entry.key;

                if (SetProgress(source, target, 0, 100) == false) return FileOperationResult.UserAbort;
                S3Service.CopyObject(bucketName, key, entry.bucketName, entry.key);

                if (move)
                {
                    if (SetProgress(source, target, 50, 100) == false) return FileOperationResult.UserAbort;
                    DeleteFile();
                }
                if (SetProgress(source, target, 100, 100) == false) return FileOperationResult.UserAbort;

                return FileOperationResult.OK;
            }
            catch (Exception ex)
            {
                Context.Log.Error(ex);
                return FileOperationResult.WriteError;
            }
        }

        public override bool CreateFolder()
        {
            if (string.IsNullOrEmpty(FolderKey)) throw new ArgumentNullException("name", "Folder name can not be null.");

            S3Service.AddObject(bucketName, FolderKey, 0, null, stream => { });
            return true;
        }

        public override bool DeleteFolder()
        {
            S3Service.DeleteObject(bucketName, FolderKey);
            return true;
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.YourSelf;
        }


        private FindData ToFindData(S3Entry entry)
        {
            var file = entry as S3File;
            return file != null ?
                new FindData(entry.Key, file.Size, file.LastModified) :
                new FindData(entry.Key, FileAttributes.Directory);
        }

        private S3Transfer GetTransfer()
        {
            var transfer = new S3Transfer(S3Service, bucketName, key);
            transfer.Error += error => Context.Log.Error(error);
            transfer.Progress += SetProgress;
            return transfer;
        }

        private bool SetProgress(string source, string target, long offset, long length)
        {
            var percent = length != 0 ? (int)(offset * 100 / length) : 100;
            return Context.Progress.SetProgress(source, target, percent);
        }
    }
}
