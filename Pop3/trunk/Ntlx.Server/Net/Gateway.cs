using System;
using System.Collections.Generic;
using log4net;
using Ntlx.Server.Collections.Generic;

namespace Ntlx.Server.Net
{
	public class Gateway
	{
		private object syncRoot = new object();

		private IDictionary<string, INetListener> listeners = new Dictionary<string, INetListener>();

		private IDictionary<string, string> connectionListenerMap = new ThreadSafeDictionary<string, string>();

		private readonly static ILog log = LogManager.GetLogger(typeof(Gateway));


		public bool Active
		{
			get;
			private set;
		}


		public void AddNetListener(INetListener listener)
		{
			if (listener == null) throw new ArgumentNullException("listener");
			lock (syncRoot)
			{
				try
				{
					if (Active) throw new InvalidOperationException("Could not add listener when gateway active.");

					listeners.Add(listener.Name, listener);
					listener.OpenNetConnection += OpenConnection;

					log.DebugFormat("Add listener '{0}'", listener.Name);
				}
				catch (Exception e)
				{
					log.ErrorFormat("Error add listener '{0}': {1}", listener.Name, e);
					throw;
				}
			}
		}

		public void RemoveXmppListener(string name)
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
						listener.OpenNetConnection -= OpenConnection;
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

		public INetConnection GetNetConnection(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) return null;

			string listenerName = null;
			if (!connectionListenerMap.TryGetValue(connectionId, out listenerName)) return null;

			INetListener listener = null;
			if (!listeners.TryGetValue(listenerName, out listener)) return null;

			return listener.GetNetConnection(connectionId);
		}

		
		private void OpenConnection(object sender, NetConnectionOpenEventArgs e)
		{
			connectionListenerMap[e.Connection.Id] = ((INetListener)sender).Name;
			e.Connection.Closed += CloseConnection;
		}

		private void CloseConnection(object sender, NetConnectionCloseEventArgs e)
		{
			var connection = (INetConnection)sender;
			connection.Closed -= CloseConnection;
			connectionListenerMap.Remove(connection.Id);
		}
	}
}
