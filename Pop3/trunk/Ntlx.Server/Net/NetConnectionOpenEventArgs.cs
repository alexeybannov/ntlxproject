using System;

namespace Ntlx.Server.Net
{
	public class NetConnectionOpenEventArgs : EventArgs
	{
		public INetConnection Connection
		{
			get;
			private set;
		}

		public NetConnectionOpenEventArgs(INetConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			Connection = connection;
		}
	}
}
