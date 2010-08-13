using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin.Wfx.FileSystem;

namespace AmazonS3Commander.Accounts
{
    class AccountManager
    {
        private readonly FileSystemContext context;

        private readonly string path;

        private readonly Encoding encoding = Encoding.Unicode;

        private const string EXT = ".s3a";


        public AccountManager(FileSystemContext context)
        {
            this.context = context;
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Resources.ProductName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        public IEnumerable<IFile> GetAccounts()
        {
            return Directory
                .GetFiles(path, "*" + EXT)
                .Select(p => (IFile)new Account(this, p, context))
                .ToList();
        }

        public AccountInfo GetAccountInfo(string name)
        {
            var path = GetPath(name);
            return File.Exists(path) ?
                AccountInfo.Parse(File.ReadAllLines(path, encoding)) :
                null;
        }

        public bool Exists(string name)
        {
            return File.Exists(GetPath(name));
        }

        public void Move(string oldName, string newName)
        {
            File.Move(GetPath(oldName), GetPath(newName));
        }

        public bool Remove(string name)
        {
            var path = GetPath(name);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        public void Save(string name, AccountInfo info)
        {
            File.WriteAllLines(GetPath(name), new[] { info.ToString() }, encoding);
        }

        private string GetPath(string name)
        {
            return Path.Combine(path, (name ?? string.Empty).Trim('\\') + EXT);
        }
    }
}
