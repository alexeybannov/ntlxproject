using System;
using System.Collections.Generic;
using log4net;
using Ntlx.Server.Collections.Generic;
using Ntlx.Server.Utils;

namespace Ntlx.Server.Net
{
	public class Gateway
	{
		private object syncRoot = new object();

		private IDictionary<string, IListener> listeners = new Dictionary<string, IListener>();

		private IDictionary<string, string> connectionListenerMap = new ThreadSafeDictionary<string, string>();

		private readonly static ILog log = LogManager.GetLogger(typeof(Gateway));


		public bool Active
		{
			get;
			private set;
		}


		public void AddListener(IListener listener)
		{
			if (listener == null) throw new ArgumentNullException("listener");
			lock (syncRoot)
			{
				try
				{
					if (Active) throw new InvalidOperationException("Could not add listener when gateway active.");

					listeners.Add(listener.Name, listener);
					listener.OpenConnection += OnOpenConnection;

					log.DebugFormat("Add listener '{0}'", listener.Name);
				}
				catch (Exception e)
				{
					log.ErrorFormat("Error add listener '{0}': {1}", listener.Name, e);
					throw;
				}
			}
		}

		public void RemoveListener(string name)
		{
			lock (syncRoot)
			{
				try
				{
					if (Active) throw new InvalidOperationException("Could not remove listener when gateway active.");
					if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

					if (listeners.ContainsKey(name))
					{
						var listener = listeners[name];
						listener.OpenConnection -= OnOpenConnection;
						listeners.Remove(name);

						log.DebugFormat("Remove listener '{0}'", listener.Name);
					}
				}
				catch (Exception e)
				{
					log.ErrorFormat("Error remove listener '{0}': {1}", name, e);
					throw;
				}
			}
		}

		public void Start()
		{
			lock (syncRoot)
			{
				foreach (var listener in listeners.Values)
				{
					try
					{
						listener.Start();
						log.DebugFormat("Started listener '{0}'", listener.Name);
					}
					catch (Exception e)
					{
						log.ErrorFormat("Error start listener '{0}': {1}", listener.Name, e);
					}
				}
				Active = true;
			}
		}

		public void Stop()
		{
			lock (syncRoot)
			{
				foreach (var listener in listeners.Values)
				{
					try
					{
						listener.Stop();
						log.DebugFormat("Stopped listener '{0}'", listener.Name);
					}
					catch (Exception e)
					{
						log.ErrorFormat("Error stop listener '{0}': {1}", listener.Name, e);
					}
				}
				Active = false;
			}
		}

		public IConnection GetConnection(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) return null;

			string listenerName = null;
			if (!connectionListenerMap.TryGetValue(connectionId, out listenerName)) return null;

			IListener listener = null;
			if (!listeners.TryGetValue(listenerName, out listener)) return null;

			return listener.GetConnection(connectionId);
		}


		public event EventHandler<ConnectionOpenEventArgs> OpenConnection;

		public event EventHandler<ConnectionCloseEventArgs> CloseConnection;

		
		private void OnOpenConnection(object sender, ConnectionOpenEventArgs e)
		{
			connectionListenerMap[e.Connection.Id] = ((IListener)sender).Name;
			e.Connection.Closed += OnCloseConnection;
			EventRaiser.SoftRaiseEvent(OpenConnection, this, e);
		}

		private void OnCloseConnection(object sender, ConnectionCloseEventArgs e)
		{
			var connection = (IConnection)sender;
			connection.Closed -= CloseConnection;
			EventRaiser.SoftRaiseEvent(CloseConnection, this, e);
			connectionListenerMap.Remove(connection.Id);
		}
	}
}
