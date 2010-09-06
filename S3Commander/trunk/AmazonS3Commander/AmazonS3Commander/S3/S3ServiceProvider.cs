using System.Collections.Generic;
using AmazonS3Commander.Accounts;

namespace AmazonS3Commander.S3
{
    class S3ServiceProvider
    {
        private readonly AccountManager accountManager;

        private readonly IDictionary<string, IS3Service> cache;


        public S3ServiceProvider(AccountManager accountManager)
        {
            this.accountManager = accountManager;
            this.cache = new Dictionary<string, IS3Service>();
        }

        public IS3Service GetS3Service(string accountName)
        {
            var key = accountName.ToLowerInvariant();

            if (cache.ContainsKey(key)) return cache[key];

            var accountInfo = accountManager.GetAccountInfo(accountName);
            cache[key] = accountInfo != null ? 
                new LitS3Service(accountInfo.AccessKey, accountInfo.SecretKey) :
                null;
            return cache[key];
        }
    }
}
