using System;
using System.Collections.Generic;
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppListener : IConfigurable
	{
		string Name
		{
			get;
			set;
		}

		void Start();

		void Stop();

		IXmppConnection GetXmppConnection(string connectionId);

		event EventHandler<XmppConnectionOpenEventArgs> OpenXmppConnection;
	}

	public class XmppConnectionOpenEventArgs : EventArgs
	{
		public IXmppConnection XmppConnection
		{
			get;
			private set;
		}

		public XmppConnectionOpenEventArgs(IXmppConnection xmppConnection)
		{
			if (xmppConnection == null) throw new ArgumentNullException("xmppConnection");

			XmppConnection = xmppConnection;
		}
	}
}
