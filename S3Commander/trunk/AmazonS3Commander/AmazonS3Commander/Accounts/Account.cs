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

        private readonly S3Service s3;

        private AccountInfo accountInfo;


        public Account(AccountManager accountManager, string file, FileSystemContext context)
        {
            this.context = context;
            this.accountManager = accountManager;
            //this.accountInfo = accountManager.GetAccountInfo(name);
            this.s3 = new S3Service();
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (OperationContext.Operation != StatusOperation.List) return null;

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
            icon = Icons.Account;
            return CustomIconResult.Extracted;
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
