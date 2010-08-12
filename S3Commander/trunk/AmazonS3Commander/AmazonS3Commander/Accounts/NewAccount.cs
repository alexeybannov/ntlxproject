using System;
using System.Drawing;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using System.Windows.Forms;
using AmazonS3Commander.Properties;

namespace AmazonS3Commander.Accounts
{
    class NewAccount : IFile
    {
        private AccountManager accountManager;

        private Request request;


        public NewAccount(AccountManager accountManager, Request request)
        {
            this.accountManager = accountManager;
            this.request = request;
        }


        public FindData GetFileInfo()
        {
            return new FindData(Resources.NewAccount);
        }

        public ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            using (var form = new AccountForm(""))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (accountManager.Exists(form.AccountName))
                    {
                        var answer = request.MessageBox("Заменить?", MessageBoxButtons.YesNo);
                        if (!answer) return ExecuteResult.Error;
                    }
                    accountManager.Save(form.AccountName, form.AccountInfo);
                    return ExecuteResult.OK;
                }
            }
            return ExecuteResult.Error;
        }

        public ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            return Open(window, ref link);
        }

        public bool CreateFolder(string name)
        {
            throw new NotImplementedException();
        }

        public bool Remove()
        {
            throw new NotImplementedException();
        }

        public CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            throw new NotImplementedException();
        }

        public PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap)
        {
            throw new NotImplementedException();
        }
    }
}
