using System;
using System.Collections.Generic;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using AmazonS3Commander.Accounts;

namespace AmazonS3Commander
{
    class AmazonS3FileSystem : IFileSystem
    {
        private FileSystemContext context;

        public void Initialize(FileSystemContext context)
        {
            this.context = context;
        }

        public IFile ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            path = path.TrimEnd('\\');
            return null;
        }

        public IEnumerator<IFile> GetFiles(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            
            path = path.TrimEnd('\\');
            if (path == string.Empty)
            {
                var accounts = new List<IFile>();
                accounts.Add(new NewAccount(new AccountManager(), context.Request));
                return accounts.GetEnumerator();
            }
            return null;
        }

        public void StatusInfo(string path, StatusOrigin origin, StatusOperation operation)
        {

        }

        public bool Disconnect(string root)
        {
            return false;
        }
    }
}
