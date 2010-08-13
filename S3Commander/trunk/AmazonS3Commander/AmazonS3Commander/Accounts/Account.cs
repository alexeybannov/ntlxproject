using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Accounts
{
    class Account : FileBase
    {
        private readonly Icon icon;

        private readonly AccountManager accountManager;


        public Account(AccountManager accountManager, string file)
        {
            this.accountManager = accountManager;
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
            Info = new FindData(name, FileAttributes.Directory);
            Info.LastWriteTime = File.GetLastWriteTime(file);
            icon = Resources.AccountIcon;
        }


        public override IEnumerator<IFile> GetFiles()
        {
            return new List<IFile>().GetEnumerator();
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            var name = Info.FileName;
            using (var form = new AccountForm(name, accountManager.GetAccountInfo(name)))
            {
                if (form.ShowDialog() == DialogResult.OK && accountManager.Exists(form.AccountName))
                {
                    accountManager.Save(form.AccountName, form.AccountInfo);
                }
            }
            return ExecuteResult.OK;
        }

        public override bool Remove()
        {
            return accountManager.Remove(Info.FileName);
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
