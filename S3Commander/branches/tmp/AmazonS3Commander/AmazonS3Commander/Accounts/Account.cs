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

        private readonly string accountName;


        public Account(AccountManager accountManager, string accountName)
        {
            this.accountManager = accountManager;
            this.accountName = accountName;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation != StatusOperation.List) return new List<FindData>().GetEnumerator();
            return S3Service
                .GetBuckets()
                .Select(b => new FindData(b.Name, FileAttributes.Directory) { LastWriteTime = b.CreationDate })
                .GetEnumerator();
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            using (var form = new AccountForm(accountName, accountManager.GetAccountInfo(accountName)))
            {
                if (form.ShowDialog() == DialogResult.OK && accountManager.Exists(accountName))
                {
                    accountManager.Save(accountName, form.AccountInfo);
                }
            }
            return ExecuteResult.OK;
        }

        public override bool DeleteFolder()
        {
            return accountManager.Remove(accountName);
        }

        public override Icon GetIcon()
        {
            return Icons.Account;
        }
    }
}
