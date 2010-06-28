using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace S3CmdPlugin.Accounts
{
	class AccountManager
	{
		private string path;
		private const string EXT = ".s3a";

		public AccountManager(string path)
		{
			this.path = path;
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		}

		public IEnumerable<string> GetAccounts()
		{
			return Directory
				.GetFiles(path, "*" + EXT)
				.Select(p => Path.GetFileNameWithoutExtension(p));
		}

		public bool GetAccount(string name, out string accessKey, out string secretKey)
		{
			accessKey = secretKey = string.Empty;
			if (File.Exists(GetPath(name)))
			{
				var lines = File.ReadAllLines(GetPath(name));
				if (0 < lines.Length) accessKey = lines[0];
				if (1 < lines.Length) secretKey = lines[1];
			}
			return false;
		}

		public void Save(string name, string accessKey, string secretKey)
		{
			File.WriteAllLines(GetPath(name), new[] { accessKey, secretKey });
		}

		public void Move(string oldName, string newName)
		{
			File.Move(GetPath(oldName.Trim('\\')), GetPath(newName.Trim('\\')));
		}

		public bool Remove(string name)
		{
			if (File.Exists(GetPath(name)))
			{
				File.Delete(GetPath(name));
				return true;
			}
			return false;
		}

		private string GetPath(string name)
		{
			return Path.Combine(path, name + EXT);
		}
	}
}
