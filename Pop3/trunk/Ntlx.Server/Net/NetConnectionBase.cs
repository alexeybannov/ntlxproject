using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntlx.Server.Net
{
	public abstract class NetConnectionBase : INetConnection
	{
		public string Id
		{
			get;
			private set;
		}


		public NetConnectionBase(string id)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
			Id = id;
		}


		public abstract void BeginReceive();

		public abstract void Send(byte[] buffer, int offset, int count);

		public abstract void SendAsync(byte[] buffer, int offset, int count);

		public void Close(NetConnectionCloseReason reason)
		{
			throw new NotImplementedException();
		}

	
		public event EventHandler<NetConnectionCloseEventArgs> Closed;

		public abstract event EventHandler Received;

		public abstract event EventHandler Sended;
	}
}
