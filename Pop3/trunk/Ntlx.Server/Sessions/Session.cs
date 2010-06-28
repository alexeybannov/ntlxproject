using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntlx.Server.Sessions
{
	public class Session
	{
		public string Id
		{
			get;
			private set;
		}

		public Session(string id)
		{
			Id = id;
		}
	}
}
