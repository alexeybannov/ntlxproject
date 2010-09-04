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
            return S3Service
                .GetObjects(bucketName, FolderKey)
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

        public override bool CreateFolder(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "Folder name can not be null.");

            S3Service.AddObject(bucketName, FolderKey + name + "/", 0, null, stream => { });
            return true;
        }

        public override bool DeleteFolder()
        {
            S3Service.DeleteObject(bucketName, FolderKey);
            return true;
        }

        public override bool DeleteFile()
        {
            S3Service.DeleteObject(bucketName, key);
            return true;
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.YourSelf;
        }


        private FindData ToFindData(S3Entry entry)
        {
            var index = entry.Key.TrimEnd('/').LastIndexOf('/');
            var name = 0 <= index ? entry.Key.Substring(index + 1) : entry.Key;

            var file = entry as S3File;
            if (file != null)
            {
                return new FindData(name, file.Size)
                {
                    LastWriteTime = file.LastModified
                };
            }
            return new FindData(name, FileAttributes.Directory);
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
