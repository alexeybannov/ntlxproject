using System;

namespace Ntlx.Server.Net
{
	public interface INetConnection
	{
		string Id
		{
			get;
		}

		
		void BeginReceive();

		void Close(NetConnectionCloseReason reason);

		void Send(byte[] buffer, int offset, int count);

		void SendAsync(byte[] buffer, int offset, int count);

		
		event EventHandler<NetConnectionCloseEventArgs> Closed;

		event EventHandler Received;

		event EventHandler Sended;
	}
}
