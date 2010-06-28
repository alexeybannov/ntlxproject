using System;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;
using System.Windows.Forms;

namespace S3CmdPlugin.Accouts
{
	class NewAccount : IFindDataProvider, IFile
	{
		private AccountManager accountManager;
		private FindData findData;

		public FindData FindData
		{
			get { return findData; }
		}

		public NewAccount(AccountManager accountManager)
		{
			this.accountManager = accountManager;
			findData.FileName = PluginResources.NewAccount;
		}

		public ExecExitCode Open(IntPtr mainWin)
		{
			using (var form = new AccountForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					accountManager.Save(form.AccountName, form.AccountAccessKey, form.AccountSecretKey);
					new MainWindow(mainWin).Refresh();
				}
			}
			return ExecExitCode.OK;
		}

		public ExecExitCode Properties(IntPtr mainWin)
		{
			return ExecExitCode.OK;
		}

		public ExecExitCode ChMod(IntPtr mainWin, string mod)
		{
			return ExecExitCode.OK;
		}

		public ExecExitCode Quote(IntPtr mainWin, string command)
		{
			return ExecExitCode.OK;
		}
	}
}
