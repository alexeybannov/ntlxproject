using System;
using System.Collections.Generic;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Configuration;
using AmazonS3Commander.Properties;
using AmazonS3Commander.S3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using System.Diagnostics;

namespace AmazonS3Commander
{
    class AmazonS3FileSystem : FileBase, IFileSystem
    {
        private readonly FileSystemContext context;

        private readonly AccountManager accountManager;

        private readonly S3ServiceProvider s3ServiceProvider;

        private readonly IFile newAccount;

        private readonly IFile config;


        public AmazonS3FileSystem(FileSystemContext context)
        {
            this.context = context;
            accountManager = new AccountManager();
            s3ServiceProvider = new S3ServiceProvider(accountManager);
            newAccount = new NewAccount(accountManager, context);
            config = new ConfigurationFile();
        }


        public IFile ResolvePath(string path)
        {
            if (path == null) return null;
            path = path.TrimEnd('\\');

            var parts = path.Split('\\');
            var depth = parts.Length - 1;

            //root
            if (depth == 0) return this;

            if (parts[depth] == "..") return null;

            var accountName = parts[1];
            
            //accounts
            if (depth == 1)
            {
                if (accountName.Equals(Resources.Settings, StringComparison.InvariantCultureIgnoreCase)) return config;
                if (accountName.Equals(Resources.NewAccount, StringComparison.InvariantCultureIgnoreCase)) return newAccount;
                return accountManager.Exists(accountName) ? new Account(accountName, accountManager, s3ServiceProvider, context) : null;
            }

            var bucketName = parts[2];

            //buckets
            if (depth == 2)
            {
                return new S3Bucket(s3ServiceProvider.GetS3Service(accountName), bucketName);
            }

            //amazon s3 folders
            if (3 <= depth)
            {
                return new S3Folder(s3ServiceProvider.GetS3Service(accountName), bucketName, string.Join("/", parts, 3, depth - 2));
            }

            return null;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            return accountManager
                .GetAccounts()
                .Union(new[] { new FindData(Resources.NewAccount), new FindData(Resources.Settings) })
                .GetEnumerator();
        }

        public override bool CreateFolder(string name)
        {
            return newAccount.CreateFolder(name);
        }


        public void OperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            Debug.WriteLine(string.Format("Operation {0} for directory '{1}' {2}", operation, remoteDir, origin.ToString().ToLower()));
            OperationContext.ProcessOperationInfo(remoteDir, origin, operation);
        }

        public bool Disconnect(string root)
        {
            return false;
        }
    }
}
