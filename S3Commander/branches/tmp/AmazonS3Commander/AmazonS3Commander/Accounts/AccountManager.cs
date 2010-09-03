using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander.Accounts
{
    class AccountManager
    {
        private readonly string path;

        private readonly Encoding encoding = Encoding.Unicode;

        private const string EXT = ".s3a";


        public AccountManager(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            this.path = path;
        }


        public IEnumerable<FindData> GetAccounts()
        {
            return Directory
                .GetFiles(path, "*" + EXT)
                .Select(file =>
                {
                    return new FindData(Path.GetFileNameWithoutExtension(file), FileAttributes.Directory)
                    {
                        LastWriteTime = File.GetLastWriteTime(file)
                    };
                });
        }

        public AccountInfo GetAccountInfo(string name)
        {
            var path = GetPath(name);
            return File.Exists(path) ? AccountInfo.Parse(File.ReadAllLines(path, encoding)) : null;
        }

        public void Save(string name, AccountInfo info)
        {
            File.WriteAllLines(GetPath(name), new[] { info.ToString() }, encoding);
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

        private string GetPath(string name)
        {
            return Path.Combine(path, (name ?? string.Empty).Trim('\\') + EXT);
        }
    }
}
