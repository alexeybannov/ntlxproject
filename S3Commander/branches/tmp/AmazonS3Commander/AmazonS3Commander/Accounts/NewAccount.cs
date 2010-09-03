using System;
using System.Drawing;
using System.Windows.Forms;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Accounts
{
    class NewAccount : S3CommanderFile
    {
        private readonly AccountManager accountManager;


        public NewAccount(AccountManager accountManager)
        {
            this.accountManager = accountManager;
        }


        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            if (CreateFolder(string.Empty))
            {
                window.Refresh();
            }
            return ExecuteResult.OK;
        }

        public override bool CreateFolder(string name)
        {
            using (var form = new AccountForm(name))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                if (form.AccountName.Equals(Resources.NewAccount, StringComparison.InvariantCultureIgnoreCase) ||
                    form.AccountName.Equals(Resources.Settings, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (accountManager.Exists(form.AccountName) &&
                    Context.Request.MessageBox(string.Format(Resources.ReplaceAccount, form.AccountName), MessageBoxButtons.YesNo) == false)
                {
                    return false;
                }
                accountManager.Save(form.AccountName, form.AccountInfo);
                return true;
            }
        }

        public override Icon GetIcon()
        {
            return Icons.NewAccount;
        }
    }
}
