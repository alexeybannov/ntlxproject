using System.Text;

namespace AmazonS3Commander.Accounts
{
	class AccountInfo
	{
		public string AccessKey
		{
			get;
			private set;
		}

		public string SecretKey
		{
			get;
            private set;
		}

		public AccountInfo()
		{

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
			var account = new AccountInfo();
			if (info != null && 0 < info.Length) account.AccessKey = info[0];
			if (info != null && 1 < info.Length) account.SecretKey = info[1];
			return account;
		}
	}
}
