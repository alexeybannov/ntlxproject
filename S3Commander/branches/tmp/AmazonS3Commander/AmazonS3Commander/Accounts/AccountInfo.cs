using System;
using System.Text;

namespace AmazonS3Commander.Accounts
{
    [Serializable]
    class AccountInfo
    {
        public string AccessKey
        {
            get;
            set;
        }

        public string SecretKey
        {
            get;
            set;
        }

        public AccountInfo(string accessKey, string secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(AccessKey)
                .AppendLine(SecretKey)
                .ToString()
                .Trim();
        }

        public static AccountInfo Parse(string[] info)
        {
            return new AccountInfo(
                info != null && 0 < info.Length ? info[0] : null,
                info != null && 1 < info.Length ? info[1] : null
            );
        }
    }
}
