using System.Collections;
using System.Windows.Forms;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Accounts
{
	class Account : FileBase, IDirectory
	{
		private AccountManager accountManager;


		public Account(AccountManager accountManager, string name)
		{
			this.accountManager = accountManager;
			findData.FileName = name;
			findData.Attributes = FileAttributes.Directory;
		}


		public bool Create(PluginContext context, string directory)
		{
			return false;
		}

		public override bool Delete(PluginContext context)
		{
			accountManager.Remove(FindData.FileName);
			return true;
		}

		public override FileSystemExitCode Move(string newName, bool overwrite, RemoteInfo info, PluginContext context)
		{
			accountManager.Move(FindData.FileName, newName);
			return FileSystemExitCode.OK;
		}

		public override ExecExitCode Properties(PluginContext context)
		{
			var accessKey = string.Empty;
			var secretKey = string.Empty;
			accountManager.GetAccount(FindData.FileName, out accessKey, out secretKey);
			using (var form = new AccountForm(FindData.FileName, accessKey, secretKey))
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					accountManager.Save(form.AccountName, form.AccountAccessKey, form.AccountSecretKey);
				}
			}
			return ExecExitCode.OK;
		}

		public IFile Current
		{
			get { return null; }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{

		}

		public void Dispose()
		{

		}
	}
}
