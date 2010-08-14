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

        private readonly Icon icon16x16;
        private readonly Icon icon32x32;

        private readonly S3Service s3;

        private AccountInfo accountInfo;

        private bool open = false;


        public Account(AccountManager accountManager, string file, FileSystemContext context)
        {
            this.context = context;
            this.accountManager = accountManager;
            this.icon16x16 = Resources.AccountIcon;
            this.icon32x32 = Resources.AccountIcon32x32;

            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(file));
            Info = new FindData(name, FileAttributes.Directory)
            {
                LastWriteTime = File.GetLastWriteTime(file)
            };

            this.accountInfo = accountManager.GetAccountInfo(name);
            this.s3 = new S3Service();
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (!open) return null;

            s3.AccessKeyID = accountInfo.AccessKey;
            s3.SecretAccessKey = accountInfo.SecretKey;

            return s3
                .GetAllBuckets()
                .Select(b => ToFindData(b))
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
                icon = icon16x16;
                return CustomIconResult.Extracted;
            }
            if (extractIconFlag == CustomIconFlag.Large)
            {
                icon = icon32x32;
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

        private FindData ToFindData(Bucket bucket)
        {
            return new FindData(bucket.Name, FileAttributes.Directory)
            {
                LastWriteTime = bucket.CreationDate
            };
        }
    }
}
