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
        private AccountManager accountManager;

        private Request request;

        private Icon icon;


        public NewAccount(AccountManager accountManager, Request request)
        {
            this.accountManager = accountManager;
            this.request = request;
            this.icon = Resources.NewAccountIcon;
        }


        public override FindData GetFileInfo()
        {
            return new FindData(Resources.NewAccount);
        }

        public override ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            if (CreateFolder(string.Empty))
            {
                window.Refresh();
                return ExecuteResult.OK;
            }
            return ExecuteResult.Error;
        }

        public override bool CreateFolder(string name)
        {
            using (var form = new AccountForm(name))
            {
                if (form.ShowDialog() != DialogResult.OK || string.Compare(form.AccountName, Resources.NewAccount, true) == 0) return false;
                if (accountManager.Exists(form.AccountName))
                {
                    var replace = request.MessageBox(
                        string.Format(Resources.ReplaceAccount, form.AccountName),
                        MessageBoxButtons.YesNo
                    );
                    if (!replace) return false;
                }
                accountManager.Save(form.AccountName, form.AccountInfo);
                return true;
            }
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            if (extractIconFlag == CustomIconFlag.Small)
            {
                icon = this.icon;
                return CustomIconResult.Extracted;
            }
            return CustomIconResult.UseDefault;
        }
    }
}
