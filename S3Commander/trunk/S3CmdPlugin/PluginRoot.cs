using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using S3CmdPlugin.Accouts;
using S3CmdPlugin.Resources;
using System.Windows.Forms;

namespace S3CmdPlugin
{
    class PluginRoot : IPathResolver, IDirectory
    {
        private List<IFindDataProvider> items;
        private IEnumerator<IFindDataProvider> enumerator;
        private AccountManager accountManager;


        public PluginRoot()
        {
            Reset();
        }

        public object ResolvePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (path == string.Empty) return null;

            if (path == "\\") return this;

            if (path[0] == '\\') path = path.Substring(1);
            var position = path.IndexOf('\\');
            var name = 0 < position ? path.Substring(0, position) : path;

            var node = items.Find(i => i.FindData.FileName == name);
            return 0 < position && node is IPathResolver ?
                ((IPathResolver)node).ResolvePath(path.Substring(position)) :
                node;
        }


        public bool Create(string directory)
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

        public bool Remove()
        {
            return false;
        }


        public IFindDataProvider Current
        {
            get { return enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Reset()
        {
            items = new List<IFindDataProvider>();
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
        }
    }
}
