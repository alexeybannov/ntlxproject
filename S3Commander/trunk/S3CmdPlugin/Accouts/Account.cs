using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.TotalCommanderT;
using System.Collections;

namespace S3CmdPlugin.Accouts
{
	class Account : IFindDataProvider, IDirectory
	{
		private AccountManager accountManager;
		private FindData findData;

		public FindData FindData
		{
			get { return findData; }
		}

		public Account(AccountManager accountManager, string name)
		{
			this.accountManager = accountManager;
			findData.FileName = name;
			findData.Attributes = FileAttributes.Directory;
		}

        public bool Create(string directory)
        {
            return false;
        }

        public bool Remove()
        {
            accountManager.Remove(findData.FileName);
            return true;
        }

        public IFindDataProvider Current
        {
            get { return null; }
        }

        public void Dispose()
        {
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
    }
}
