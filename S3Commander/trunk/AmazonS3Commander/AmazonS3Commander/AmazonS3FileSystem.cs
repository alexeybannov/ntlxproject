using System;
using System.Collections.Generic;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Configuration;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander
{
    class AmazonS3FileSystem : FileBase, IFileSystem
    {
        private readonly FileSystemContext context;

        private readonly AccountManager accountManager;

        private readonly IFile newAccount;

        private readonly IFile config;


        public AmazonS3FileSystem(FileSystemContext context)
        {
            this.context = context;
            accountManager = new AccountManager(context);
            newAccount = new NewAccount(accountManager, context.Request);
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

            //accounts
            if (depth == 1)
            {
                if (EqualStrings(parts[depth], Resources.Settings)) return config;
                if (EqualStrings(parts[depth], Resources.NewAccount)) return newAccount;
                return accountManager.GetAccount(parts[depth]);
            }

            //buckets
            if (depth == 2)
            {

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


        private bool EqualStrings(string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
