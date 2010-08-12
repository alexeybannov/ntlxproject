using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Windows.Forms;
using AmazonS3Commander.Properties;

namespace AmazonS3Commander.Accounts
{
    class AccountManager
    {
        private string path;
        private const string EXT = ".s3a";

        public AccountManager()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Resources.ProductName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public IEnumerable<string> GetAccounts()
        {
            return Directory
                .GetFiles(path, "*" + EXT)
                .Select(p => Path.GetFileNameWithoutExtension(p));
        }

        public bool CreateNewAccountForm(string name)
        {
            using (var form = new AccountForm(name))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var path = GetPath(form.AccountName);
                    if (File.Exists(path))
                    {
                    }
                    Save(path, form.AccountInfo);
                    return true;
                }
            }
            return false;
        }

        public bool EditAccountForm(string name)
        {
            var path = GetPath(name);
            if (File.Exists(path))
            {
                var account = AccountInfo.Parse(File.ReadAllLines(path, Encoding.Unicode));
                using (var form = new AccountForm(name, account))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        Save(path, form.AccountInfo);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Exists(string name)
        {
            return true;
        }

        public void Move(string oldName, string newName)
        {
            File.Move(GetPath(oldName), GetPath(newName));
        }

        public bool Remove(string name)
        {
            if (File.Exists(GetPath(name)))
            {
                File.Delete(GetPath(name));
                return true;
            }
            return false;
        }

        public void Save(string name, AccountInfo info)
        {
            File.WriteAllLines(path, new[] { info.ToString() }, Encoding.Unicode);
        }

        private string GetPath(string name)
        {
            return Path.Combine(path, (name ?? string.Empty).Trim('\\') + EXT);
        }
    }
}
