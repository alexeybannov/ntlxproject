using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmazonS3Commander.Resources;
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
            if (accountManager == null) throw new ArgumentNullException("accountManager");
            if (accountName == null) throw new ArgumentNullException("accountName");

            this.accountManager = accountManager;
            this.accountName = accountName;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (Context.CurrentOperation == StatusOperation.List)
            {
                return S3Service
                    .GetBuckets()
                    .Select(b => new FindData(b.Key, FileAttributes.Directory, b.CreationDate))
                    .GetEnumerator();
            }
            if (Context.CurrentOperation == StatusOperation.Delete)
            {
                return EmptyFindDataEnumerator;
            }
            return null;
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

        public override bool CreateFolder()
        {
            using (var form = new AccountForm(accountName))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                if (form.AccountName.Equals(RS.NewAccount, StringComparison.InvariantCultureIgnoreCase) ||
                    form.AccountName.Equals(RS.Settings, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
                if (accountManager.Exists(form.AccountName) &&
                    Context.Request.MessageBox(string.Format(RS.ReplaceAccount, form.AccountName), MessageBoxButtons.YesNo) == false)
                {
                    return false;
                }
                accountManager.Save(form.AccountName, form.AccountInfo);
                return true;
            }
        }

        public override bool DeleteFolder()
        {
            return accountManager.Delete(accountName);
        }

        public override Icon GetIcon()
        {
            return Icons.Account;
        }
    }
}
