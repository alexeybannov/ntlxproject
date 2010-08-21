using System;
using System.Collections.Generic;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Configuration;
using AmazonS3Commander.Properties;
using AmazonS3Commander.S3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

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

            var name = parts[depth];
            if (name == "..") return null;

            //accounts
            if (depth == 1)
            {
                if (name.Equals(Resources.Settings, StringComparison.InvariantCultureIgnoreCase)) return config;
                if (name.Equals(Resources.NewAccount, StringComparison.InvariantCultureIgnoreCase)) return newAccount;
                return accountManager.Exists(name) ? new Account(name, accountManager, s3ServiceProvider, context) : null;
            }

            //buckets
            if (depth == 2)
            {
                return new BucketFile(s3ServiceProvider.GetS3Service(parts[1]), name);
            }

            //amazon s3 folders
            if (depth == 3)
            {

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
            OperationContext.ProcessOperationInfo(remoteDir, origin, operation);
        }

        public bool Disconnect(string root)
        {
            return false;
        }
    }
}
