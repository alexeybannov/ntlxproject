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

        public ExecExitCode Open(MainWindow mainWindow)
        {
            using (var form = new AccountForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    accountManager.Save(form.AccountName, form.AccountAccessKey, form.AccountSecretKey);
                    mainWindow.Refresh();
                }
            }
            return ExecExitCode.OK;
        }

        public ExecExitCode Properties(MainWindow mainWindow)
        {
            return ExecExitCode.OK;
        }

        public ExecExitCode ChMod(MainWindow mainWindow, string mod)
        {
            return ExecExitCode.OK;
        }

        public ExecExitCode Quote(MainWindow mainWindow, string command)
        {
            return ExecExitCode.OK;
        }

        public bool Delete()
        {
            return false;
        }
    }
}
