using System;
using System.Collections.Generic;
using Ntlx.Server.Collections.Generic;
using Ntlx.Server.Utils;

namespace Ntlx.Server.Net
{
	public abstract class NetListenerBase : INetListener
	{
		private IDictionary<string, INetConnection> connections = new ThreadSafeDictionary<string, INetConnection>(1000);


		public bool Listening
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			set;
		}

		public virtual void Configure(IDictionary<string, string> properties)
		{

		}

		public void Start()
		{
			lock (this)
			{
				if (Listening) return;
				Listening = true;
				
				DoStart();
			}
		}

		public void Stop()
		{
			lock (this)
			{
				if (!Listening) return;
				Listening = false;
				
				DoStop();
				
				new List<INetConnection>(connections.Values)
					.ForEach(c => c.Close(NetConnectionCloseReason.ServerShutdown));
			}
		}

		public INetConnection GetNetConnection(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) return null;
			INetConnection connection = null;
			return connections.TryGetValue(connectionId, out connection) ? connection : null;
		}


		protected abstract void DoStart();

		protected abstract void DoStop();


		public event EventHandler<NetConnectionOpenEventArgs> OpenNetConnection;


		protected void AddNewXmppConnection(INetConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			connections.Add(connection.Id, connection);
			connection.Closed += ConnectionClosed;
			EventRaiser.SoftRaiseEvent(OpenNetConnection, this, new NetConnectionOpenEventArgs(connection));
			connection.BeginReceive();
		}

		private void ConnectionClosed(object sender, NetConnectionCloseEventArgs e)
		{
			e.Connection.Closed -= ConnectionClosed;
			connections.Remove(e.Connection.Id);
		}
	}
}