using System.Windows.Forms;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Accounts
{
    class NewAccount : FileBase
    {
        private AccountManager accountManager;

		
		public NewAccount(AccountManager accountManager)
        {
            this.accountManager = accountManager;
            findData.FileName = PluginResources.NewAccount;
        }

		public override ExecExitCode Open(PluginContext context)
        {
            using (var form = new AccountForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    accountManager.Save(form.AccountName, form.AccountAccessKey, form.AccountSecretKey);
                    context.RefreshMainWindow();
                }
            }
            return ExecExitCode.OK;
        }
    }
}
