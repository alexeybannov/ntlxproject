using System.Collections;
using System.Drawing;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Accounts
{
    class Account : FileBase, IDirectory
    {
        private AccountManager accountManager;
        private string name;


        public Account(AccountManager accountManager, string name)
        {
            this.accountManager = accountManager;
            this.name = name;
            findData.FileName = name;
            findData.Attributes = FileAttributes.Directory;
        }


        public bool Create(string directory, PluginContext context)
        {
            return false;
        }

        public override bool Delete(PluginContext context)
        {
            accountManager.Remove(name);
            return true;
        }

        public override FileSystemExitCode Move(string newName, bool overwrite, RemoteInfo info, PluginContext context)
        {
            accountManager.Move(name, newName);
            return FileSystemExitCode.OK;
        }

        public override ExecExitCode Properties(PluginContext context)
        {
            accountManager.EditAccountForm(name, context);
            return ExecExitCode.OK;
        }

        public override IconExtractResult ExctractCustomIcon(IconExtractFlags ExtractFlags, ref Icon icon)
        {
            icon = PluginResources.keys;
            return IconExtractResult.ExtractedDestroy;
        }

        public IFile Current
        {
            get { return null; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {

        }

        public void Dispose()
        {

        }
    }
}
