using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Accouts
{
	class Account : IFindDataProvider
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
	}
}
