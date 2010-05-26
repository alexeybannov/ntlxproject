using System;
using Ntlx.Server.Formatters;

namespace Ntlx.Server.Net
{
	public class ConnectionBufferEventArgs : EventArgs
	{
		public IConnection Connection
		{
			get;
			private set;
		}

		public byte[] Buffer
		{
			get;
			private set;
		}

		public int Offset
		{
			get;
			private set;
		}

		public int Count
		{
			get;
			private set;
		}

		public ConnectionBufferEventArgs(IConnection connection, byte[] buffer, int offset, int count)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			Connection = connection;
			Buffer = buffer;
			Offset = offset;
			Count = count;
		}
	}
}
