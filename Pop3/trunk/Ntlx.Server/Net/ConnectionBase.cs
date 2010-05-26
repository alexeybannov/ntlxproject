using System;
using Ntlx.Server.Utils;

namespace Ntlx.Server.Net
{
	public abstract class ConnectionBase : IConnection
	{
		protected bool IsClosed
		{
			get;
			private set;
		}

		public string Id
		{
			get;
			private set;
		}


		public ConnectionBase(string id)
		{
			if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
			Id = id;
			IsClosed = false;
		}


		public abstract void BeginReceive();

		public abstract void Send(byte[] buffer, int offset, int count);

		public abstract void OnClose(ConnectionCloseReason reason);


		public void Close(ConnectionCloseReason reason)
		{
			lock (this)
			{
				if (IsClosed) return;
				IsClosed = true;

				EventRaiser.SoftRaiseEvent(Closed, this, new ConnectionCloseEventArgs(this, reason));

				OnClose(reason);
			}
		}


		public event EventHandler<ConnectionCloseEventArgs> Closed;

		public event EventHandler<ConnectionBufferEventArgs> Received;


		protected void RaiseReceived(byte[] buffer, int offset, int count)
		{
			EventRaiser.SoftRaiseEvent(
				Received,
				this,
				new ConnectionBufferEventArgs(this, buffer, offset, count)
			);
		}
	}
}
