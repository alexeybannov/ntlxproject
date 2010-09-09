using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmazonS3Commander.S3;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Files
{
    class Entry : S3CommanderFile
    {
        public string BucketName
        {
            get;
            private set;
        }

        public string Key
        {
            get;
            private set;
        }

        private string FolderKey
        {
            get { return Key != string.Empty ? Key + "/" : Key; }
        }


        public Entry(string bucketName, string key)
        {
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException("bucketName");
            if (key == null) throw new ArgumentNullException("key");

            this.BucketName = bucketName;
            this.Key = key;
        }

        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation == StatusOperation.CalculateSize ||
                Context.CurrentOperation == StatusOperation.Delete ||
                Context.CurrentOperation == StatusOperation.RenameMoveMulti)
            {
                return S3Service
                    .GetObjects(BucketName, FolderKey, "")
                    .Where(o => o is S3Entry)
                    .Select(o => ToFindData(o))
                    .GetEnumerator();
            }
            return S3Service
                .GetObjects(BucketName, FolderKey, "/")
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
            S3Service.DeleteObject(BucketName, Key);
            return true;
        }

        public override FileOperationResult CopyTo(S3CommanderFile dest, bool overwrite, bool move, RemoteInfo info)
        {
            var entry = dest as Entry;
            if (entry == null) return FileOperationResult.NotSupported;

            try
            {
                if (!overwrite && S3Service.ObjectExists(entry.BucketName, entry.Key))
                {
                    return FileOperationResult.Exists;
                }

                var source = BucketName + "/" + Key;
                var target = entry.BucketName + "/" + entry.Key;

                if (SetProgress(source, target, 0, 100) == false) return FileOperationResult.UserAbort;
                S3Service.CopyObject(BucketName, Key, entry.BucketName, entry.Key);

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

            S3Service.AddObject(BucketName, FolderKey, 0, null, stream => { });
            return true;
        }

        public override bool DeleteFolder()
        {
            S3Service.DeleteObject(BucketName, FolderKey);
            return true;
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.YourSelf;
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            using (var form = new EntryForm(this))
            {
                form.ShowDialog();
            }
            return ExecuteResult.OK;
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
            var transfer = new S3Transfer(S3Service, BucketName, Key);
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
