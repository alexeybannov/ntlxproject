using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using S3CmdPlugin.Accounts;
using S3CmdPlugin.Resources;

namespace S3CmdPlugin
{
	class PluginRoot : FileBase, IPathResolver, IDirectory
    {
        private List<IFile> items;
		private IEnumerator<IFile> enumerator;
        private AccountManager accountManager;


        public PluginRoot()
        {
            Reset();
        }

        public IFile ResolvePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (path == string.Empty) return null;

            if (path == "\\") return this;

			path = path.Trim('\\');
            var position = path.IndexOf('\\');
            var name = 0 < position ? path.Substring(0, position) : path;

            var node = items.Find(i => i.FindData.FileName == name);
            return 0 < position && node is IPathResolver ?
                ((IPathResolver)node).ResolvePath(path.Substring(position)) :
                node;
        }


        public bool Create(PluginContext context, string directory)
        {
            using (var form = new AccountForm())
            {
                form.AccountName = directory;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    accountManager.Save(form.AccountName, form.AccountAccessKey, form.AccountSecretKey);
                    return true;
                }
            }
            return false;
        }


		public IFile Current
        {
            get { return enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Reset()
        {
			items = new List<IFile>();
            accountManager = new AccountManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PluginResources.ProductName));
            items.Add(new NewAccount(accountManager));
            accountManager
                .GetAccounts()
                .ToList()
                .ForEach(a => items.Add(new Account(accountManager, a)));

            enumerator = items.GetEnumerator();
            enumerator.Reset();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }


        public void Dispose()
        {
            enumerator.Dispose();
			enumerator = null;
        }
    }
}
