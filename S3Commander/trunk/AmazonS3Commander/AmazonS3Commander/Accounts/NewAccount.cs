using System;
using System.Drawing;
using AmazonS3Commander.Resources;
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
            if (window == null) throw new ArgumentNullException("window");

            var newAccount = new Account(accountManager, string.Empty).Initialize(Context);
            if (newAccount.CreateFolder())
            {
                window.Refresh();
            }
            return ExecuteResult.OK;
        }

        public override Icon GetIcon()
        {
            return Icons.NewAccount;
        }
    }
}
