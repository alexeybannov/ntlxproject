using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using S3CmdPlugin.Resources;
using System;
using System.Windows.Forms;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Accounts
{
	class AccountManager
	{
		private string path;
		private const string EXT = ".s3a";

		public AccountManager()
		{
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PluginResources.ProductName);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		}

		public IEnumerable<string> GetAccounts()
		{
			return Directory
				.GetFiles(path, "*" + EXT)
				.Select(p => Path.GetFileNameWithoutExtension(p));
		}

		public bool CreateNewAccountForm(string name, PluginContext context)
		{
			using (var form = new AccountForm(name))
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					var path = GetPath(form.AccountName);
					if (File.Exists(path))
					{
						var answer = MessageBox.Show(
							PluginResources.ReplaceAccount,
							PluginResources.ProductName,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question
						);
						if (answer != DialogResult.Yes) return false;
					}
					Save(path, form.AccountInfo);
					return true;
				}
			}
			return false;
		}

		public bool EditAccountForm(string name, PluginContext context)
		{
			var path = GetPath(name);
			if (File.Exists(path))
			{
				File.Decrypt(path);
				try
				{
					var account = AccountInfo.Parse(File.ReadAllLines(path, Encoding.Unicode));
					using (var form = new AccountForm(name, account))
					{
						if (form.ShowDialog() == DialogResult.OK)
						{
							Save(path, form.AccountInfo);
							return true;
						}
					}
				}
				finally
				{
					File.Encrypt(path);
				}
			}
			return false;
		}

		public void Move(string oldName, string newName)
		{
			File.Move(GetPath(oldName), GetPath(newName));
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

		private void Save(string path, AccountInfo info)
		{
			File.WriteAllLines(path, new[] { info.ToString() }, Encoding.Unicode);
			File.Encrypt(path);
		}

		private string GetPath(string name)
		{
			return Path.Combine(path, (name ?? string.Empty).Trim('\\') + EXT);
		}
	}
}
