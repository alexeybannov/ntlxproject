using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Configuration;
using AmazonS3Commander.Properties;
using AmazonS3Commander.S3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander
{
    class S3CommanderFileSystem : FileBase, IFileSystem
    {
        private readonly FileSystemContext fileSystemContext;

        private readonly AccountManager accountManager;

        private readonly S3ServiceProvider s3ServiceProvider;


        public S3CommanderFileSystem(FileSystemContext context)
        {
            fileSystemContext = context;
            accountManager = new AccountManager();
            s3ServiceProvider = new S3ServiceProvider(accountManager);
        }


        public IFile ResolvePath(string path)
        {
            if (path == null) return null;

            var parts = path.TrimEnd('\\').Split('\\');
            var depth = parts.Length - 1;

            //root
            if (depth <= 0)
            {
                return this;
            }

            if (parts[depth] == "..") return null;

            S3CommanderFile file = null;

            //accounts
            var accountName = parts[1];
            if (depth == 1)
            {
                if (accountName.Equals(Resources.Settings, StringComparison.InvariantCultureIgnoreCase))
                {
                    file = new ConfigurationFile();
                }
                else if (accountName.Equals(Resources.NewAccount, StringComparison.InvariantCultureIgnoreCase))
                {
                    file = new NewAccount(accountManager);
                }
                else if (accountManager.Exists(accountName))
                {
                    file = new Account(accountManager);
                }
            }
            //buckets
            else if (depth == 2)
            {
                file = new Bucket(parts[2]);
            }
            //amazon s3 folders or file
            else if (3 <= depth)
            {
                file = new Entry(parts[2], string.Join("/", parts, 3, depth - 2));
            }

            if (file != null)
            {
                file.Initialize(new S3CommanderContext(fileSystemContext, s3ServiceProvider, accountName));
            }
            return file;
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
            return new NewAccount(accountManager)
                .Initialize(new S3CommanderContext(fileSystemContext))
                .CreateFolder(name);
        }


        public void OperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            Debug.WriteLine(string.Format("Operation {0} for directory '{1}' {2}", operation, remoteDir, origin.ToString().ToLower()));
            S3CommanderContext.ProcessOperationInfo(remoteDir, origin, operation);
        }

        public bool Disconnect(string root)
        {
            return false;
        }
    }
}
