using System.Collections.Generic;
using AmazonS3Commander.Accounts;

namespace AmazonS3Commander.S3
{
    class S3ServiceProvider
    {
        private readonly AccountManager accountManager;

        private readonly IDictionary<string, S3Service> cache;


        public S3ServiceProvider(AccountManager accountManager)
        {
            this.accountManager = accountManager;
            this.cache = new Dictionary<string, S3Service>();
        }

        public S3Service GetS3Service(string accountName)
        {
            var key = accountName.ToLowerInvariant();

            if (cache.ContainsKey(key)) return cache[key];

            var accountInfo = accountManager.GetAccountInfo(accountName);
            cache[key] = new S3Service(accountInfo.AccessKey, accountInfo.SecretKey);
            return cache[key];
        }
    }
}
