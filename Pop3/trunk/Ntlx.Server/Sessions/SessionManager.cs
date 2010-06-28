using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntlx.Server.Collections.Generic;

namespace Ntlx.Server.Sessions
{
	public class SessionManager
	{
		private IDictionary<string, Session> sessions = new ThreadSafeDictionary<string, Session>();


		public void AddSession(Session session)
		{

		}

		public void RemoveSession(string id)
		{

		}
	}
}
