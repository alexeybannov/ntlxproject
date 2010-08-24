using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmazonS3Commander.S3;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Accounts
{
    class Account : FileBase
    {
        private readonly AccountManager accountManager;

        private readonly S3ServiceProvider s3ServiceProvider;

        private readonly FileSystemContext context;

        private readonly string accountName;


        public Account(string accountName, AccountManager accountManager, S3ServiceProvider s3ServiceProvider, FileSystemContext context)
        {
            this.context = context;
            this.accountManager = accountManager;
            this.s3ServiceProvider = s3ServiceProvider;
            this.accountName = accountName;
        }


        public override IEnumerator<FindData> GetFiles()
        {
            if (OperationContext.Operation != StatusOperation.List) return new List<FindData>().GetEnumerator();

            return s3ServiceProvider.GetS3Service(accountName)
                .GetAllBuckets()
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

        public override bool Delete()
        {
            return accountManager.Remove(accountName);
        }

        public override CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            icon = Icons.Account;
            return CustomIconResult.Extracted;
        }
    }
}
