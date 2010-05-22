using System;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppReceiver
	{
		event EventHandler<XmppStreamStartEventArgs> XmppStreamStart;

		event EventHandler<XmppStreamEndEventArgs> XmppStreamEnd;

		event EventHandler<XmppStreamEventArgs> XmppStreamElement;
	}
}
