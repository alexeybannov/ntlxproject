using System;

namespace Ntlx.Server.Net
{
	public interface IConnection
	{
		string Id
		{
			get;
		}

		
		void BeginReceive();

		void Close(ConnectionCloseReason reason);

		void Send(byte[] buffer, int offset, int count);

		
		event EventHandler<ConnectionCloseEventArgs> Closed;

		event EventHandler<ConnectionBufferEventArgs> Received;
	}
}
