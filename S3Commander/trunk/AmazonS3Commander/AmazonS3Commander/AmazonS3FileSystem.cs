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
        private FileSystemContext context;

        private AccountManager accountManager;

        private IFile newAccount;

        private IFile config;


        public void Initialize(FileSystemContext context)
        {
            this.context = context;
            accountManager = new AccountManager();
            newAccount = new NewAccount(accountManager, context.Request);
            config = new ConfigurationFile();
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }


        public IFile ResolvePath(string path)
        {
            if (path == null) return null;
            path = path.TrimEnd('\\');

            var depth = path.Split('\\').Length - 1;
            if (depth == 0) return this;
            
            var name = path.Substring(path.IndexOf('\\') + 1);
            if (depth == 1) return GetFirstLevel().SingleOrDefault(a => a.Info.FileName == name);

            return null;
        }


        public override IEnumerator<IFile> GetFiles()
        {
            return GetFirstLevel().GetEnumerator();
        }

        public override bool CreateFolder(string name)
        {
            return newAccount.CreateFolder(name);
        }


        public void StatusInfo(string path, StatusOrigin origin, StatusOperation operation)
        {

        }

        public bool Disconnect(string root)
        {
            return false;
        }


        private IEnumerable<IFile> GetFirstLevel()
        {
            return accountManager
                .GetAccounts()
                .Union(new[] { newAccount, config });
        }
    }
}
