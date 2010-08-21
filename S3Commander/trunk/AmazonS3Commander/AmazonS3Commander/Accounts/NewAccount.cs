using System;
using System.Drawing;
using System.Windows.Forms;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Accounts
{
    class NewAccount : FileBase
    {
        private readonly AccountManager accountManager;

        private readonly FileSystemContext context;


        public NewAccount(AccountManager accountManager, FileSystemContext context)
        {
            this.accountManager = accountManager;
            this.context = context;
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
                    context.Request.MessageBox(string.Format(Resources.ReplaceAccount, form.AccountName), MessageBoxButtons.YesNo) == false)
                {
                    return false;
                }
                accountManager.Save(form.AccountName, form.AccountInfo);
                return true;
            }
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.NewAccount;
            return CustomIconResult.Extracted;
        }
    }
}
