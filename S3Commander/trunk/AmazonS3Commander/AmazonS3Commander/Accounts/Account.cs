using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Accounts
{
    class Account : S3CommanderFile
    {
        private readonly AccountManager accountManager;


        public Account(AccountManager accountManager)
        {
            this.accountManager = accountManager;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation != StatusOperation.List) return new List<FindData>().GetEnumerator();
            return Context.S3Service
                .GetBuckets()
                .Select(b => new FindData(b.Name, FileAttributes.Directory) { LastWriteTime = b.CreationDate })
                .GetEnumerator();
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            var accountName = Context.CurrentAccount;
            using (var form = new AccountForm(accountName, accountManager.GetAccountInfo(accountName)))
            {
                if (form.ShowDialog() == DialogResult.OK && accountManager.Exists(accountName))
                {
                    accountManager.Save(accountName, form.AccountInfo);
                }
            }
            return ExecuteResult.OK;
        }

        public override bool Delete()
        {
            return accountManager.Remove(Context.CurrentAccount);
        }

        protected override Icon GetIcon()
        {
            return Icons.Account;
        }
    }
}
