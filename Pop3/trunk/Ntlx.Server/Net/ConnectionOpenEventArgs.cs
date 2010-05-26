using System;
using Ntlx.Server.Formatters;

namespace Ntlx.Server.Net
{
	public class ConnectionOpenEventArgs : EventArgs
	{
		public IConnection Connection
		{
			get;
			private set;
		}

		public IFormatter Formatter
		{
			get;
			private set;
		}

		public ConnectionOpenEventArgs(IConnection connection, IFormatter formatter)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			Connection = connection;
			Formatter = formatter;
		}
	}
}
