using System;
using System.Collections.Generic;
using System.Linq;
using AmazonS3Commander.Accounts;
using AmazonS3Commander.Buckets;
using AmazonS3Commander.Configuration;
using AmazonS3Commander.Files;
using AmazonS3Commander.Resources;
using AmazonS3Commander.S3;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    class S3CommanderRoot : S3CommanderFile
    {
        private readonly AccountManager accountManager;

        private readonly S3ServiceProvider s3ServiceProvider;


        public S3CommanderRoot(string workDirectory)
        {
            accountManager = new AccountManager(workDirectory);
            s3ServiceProvider = new S3ServiceProvider(accountManager);
        }


        public S3CommanderFile ResolvePath(string path)
        {
            S3CommanderFile file = null;

            if (path == null) return file;

            var parts = path.TrimEnd('\\').Split('\\');
            var depth = parts.Length - 1;

            //root
            if (depth <= 0)
            {
                return this;
            }

            //parent directory
            if (parts[depth] == "..") return file;

            //accounts
            var accountName = parts[1];
            if (depth == 1)
            {
                if (accountName.Equals(RS.Settings, StringComparison.InvariantCultureIgnoreCase))
                {
                    file = new ConfigurationFile();
                }
                else if (accountName.Equals(RS.NewAccount, StringComparison.InvariantCultureIgnoreCase))
                {
                    file = new NewAccount(accountManager);
                }
                else
                {
                    file = new Account(accountManager, accountName);
                }
            }
            //buckets
            else if (depth == 2)
            {
                file = new Bucket(parts[2]);
            }
            //amazon s3 folder or file
            else
            {
                file = new Entry(parts[2], string.Join("/", parts, 3, depth - 2));
            }

            file.Initialize(Context, s3ServiceProvider.GetS3Service(accountName));
            return file;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation != StatusOperation.List) return EmptyFindDataEnumerator;

            return accountManager
                .GetAccounts()
                .Union(new[] { new FindData(RS.NewAccount), new FindData(RS.Settings) })
                .GetEnumerator();
        }
    }
}
