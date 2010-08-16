using System.Collections.Generic;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Configuration;
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

        private IEnumerable<IFile> firstLevel;


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

            var depth = path.Split('\\').Length - 1;
            if (depth == 0) return this;

            var name = path.Substring(path.IndexOf('\\') + 1);
            //if (depth == 1) return GetFirstLevel().SingleOrDefault(a => a. == name);

            return null;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            //firstLevel = null;
            //return GetFirstLevel().GetEnumerator();
            return null;
        }

        public override bool CreateFolder(string name)
        {
            return newAccount.CreateFolder(name);
        }


        public void OperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            if (origin == StatusOrigin.Start) OperationContext.OperationBegin(remoteDir, operation);
            if (origin == StatusOrigin.End) OperationContext.OperationEnd();
        }

        public bool Disconnect(string root)
        {
            return false;
        }


        private IEnumerable<IFile> GetFirstLevel()
        {
            if (firstLevel == null)
            {
                firstLevel = accountManager
                .GetAccounts()
                .Union(new[] { newAccount, config });
            }
            return firstLevel;
        }
    }
}
