using System.Collections.Generic;
using System.IO;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using System.Windows.Forms;
using System.Drawing;
using AmazonS3Commander.Properties;

namespace AmazonS3Commander.Accounts
{
    class Account : FileBase
    {
        private FindData findData;

        private Icon icon;

        private AccountManager accountManager;


        public Account(AccountManager accountManager, string file)
        {
            this.accountManager = accountManager;
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
            findData = new FindData(name, FileAttributes.Directory);
            findData.LastWriteTime = File.GetLastWriteTime(file);
            icon = Resources.AccountIcon;
        }


        public override FindData GetFileInfo()
        {
            return findData;
        }

        public override IEnumerator<IFile> GetFiles()
        {
            return new List<IFile>().GetEnumerator();
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            var name = findData.FileName;
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
            return accountManager.Remove(findData.FileName);
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
