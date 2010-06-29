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
			if (accountManager.CreateNewAccountForm(string.Empty, context))
			{
				context.RefreshMainWindow();
			}
			return ExecExitCode.OK;
		}

		public override IconExtractResult ExctractCustomIcon(IconExtractFlags ExtractFlags, ref System.Drawing.Icon icon)
		{
			icon = PluginResources.new2;
			return IconExtractResult.Extracted;
		}
	}
}
