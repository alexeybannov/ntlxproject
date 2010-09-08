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

            var accountName = parts[1];
            var bucketName = 2 <= depth ? parts[2] : null;

            //accounts            
            if (depth == 1)
            {
                if (RS.Settings.Equals(accountName, StringComparison.CurrentCultureIgnoreCase))
                {
                    file = new ConfigurationFile();
                }
                else if (RS.NewAccount.Equals(accountName, StringComparison.CurrentCultureIgnoreCase))
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
                if (RS.NewBucket.Equals(bucketName, StringComparison.CurrentCultureIgnoreCase))
                {
                    file = new NewBucket();
                }
                else
                {
                    file = new Bucket(bucketName);
                }
            }
            //amazon s3 folder or file
            else
            {
                file = new Entry(bucketName, string.Join("/", parts, 3, depth - 2));
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
