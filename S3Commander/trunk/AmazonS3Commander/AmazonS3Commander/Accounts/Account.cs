using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmazonS3Commander.Properties;
using AmazonS3Commander.S3;
using LitS3;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Accounts
{
    class Account : FileBase
    {
        private readonly AccountManager accountManager;

        private readonly FileSystemContext context;

        private readonly Icon icon;

        private readonly S3Service s3;

        private AccountInfo accountInfo;

        private bool open = false;


        public Account(AccountManager accountManager, string file, FileSystemContext context)
        {
            this.context = context;
            this.accountManager = accountManager;
            this.icon = Resources.AccountIcon;

            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
            Info = new FindData(name, FileAttributes.Directory)
            {
                LastWriteTime = File.GetLastWriteTime(file)
            };

            this.accountInfo = accountManager.GetAccountInfo(name);
            this.s3 = new S3Service();
        }


        public override IEnumerator<IFile> GetFiles()
        {
            if (!open) return null;

            s3.AccessKeyID = accountInfo.AccessKey;
            s3.SecretAccessKey = accountInfo.SecretKey;

            return s3
                .GetAllBuckets()
                .Select(b => (IFile)new BucketFile(b))
                .GetEnumerator();
        }

        public override ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            using (var form = new AccountForm(Info.FileName, accountInfo))
            {
                if (form.ShowDialog() == DialogResult.OK && accountManager.Exists(form.AccountName))
                {
                    accountInfo = form.AccountInfo;
                    accountManager.Save(form.AccountName, accountInfo);
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

        public override void OperationBegin(StatusOperation operation)
        {
            open = operation == StatusOperation.List;
        }

        public override void OperationEnd(StatusOperation operation)
        {
            open = operation != StatusOperation.List;
        }
    }
}
