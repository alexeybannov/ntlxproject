using System;
using System.Collections.Generic;
using Ntlx.Server.Collections.Generic;
using Ntlx.Server.Utils;
using Ntlx.Server.Formatters;

namespace Ntlx.Server.Net
{
	public abstract class ListenerBase : IListener
	{
		private IDictionary<string, IConnection> connections = new ThreadSafeDictionary<string, IConnection>(1000);

		private Type formatterType;
		

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

		public Type FormatterType
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

				new List<IConnection>(connections.Values)
					.ForEach(c => c.Close(ConnectionCloseReason.ServerShutdown));
			}
		}

		public IConnection GetConnection(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) return null;
			IConnection connection = null;
			return connections.TryGetValue(connectionId, out connection) ? connection : null;
		}
		

		protected abstract void DoStart();

		protected abstract void DoStop();


		public event EventHandler<ConnectionOpenEventArgs> OpenConnection;


		protected void AddNewConnection(IConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			connections.Add(connection.Id, connection);
			connection.Closed += ConnectionClosed;
			EventRaiser.SoftRaiseEvent(OpenConnection, this, new ConnectionOpenEventArgs(connection, (IFormatter)Activator.CreateInstance(formatterType)));
			connection.BeginReceive();
		}

		private void ConnectionClosed(object sender, ConnectionCloseEventArgs e)
		{
			e.Connection.Closed -= ConnectionClosed;
			connections.Remove(e.Connection.Id);
		}
	}
}