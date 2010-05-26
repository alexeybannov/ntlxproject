using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntlx.Server.Transport
{
	public interface IReceiver
	{
		event EventHandler OpenConnection;

		event EventHandler CloseConnection;

		event EventHandler Received;
	}
}
