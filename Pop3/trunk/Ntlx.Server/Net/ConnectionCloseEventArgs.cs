using System;

namespace Ntlx.Server.Net
{
	public class ConnectionCloseEventArgs : EventArgs
	{
		public IConnection Connection
		{
			get;
			private set;
		}
		
		public ConnectionCloseReason Reason
		{
			get;
			private set;
		}

		public ConnectionCloseEventArgs(IConnection connection, ConnectionCloseReason reason)
		{
			Connection = connection;
			Reason = reason;
		}
	}
}
