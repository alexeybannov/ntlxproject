using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using agsXMPP.Xml.Dom;
using log4net;
using Uri = agsXMPP.Uri;

namespace ASC.Xmpp.Server.Gateway
{
	class XmppSender : IXmppSender
	{
		private XmppGateway gateway;

		private static readonly ILog log = LogManager.GetLogger(typeof(XmppSender));

		private XmppXMLSchemaValidator validator = new XmppXMLSchemaValidator();

		private const string SEND_FORMAT = "Xmpp stream: connection {0}, namespace {1}\r\n\r\n(S) -------------------------------------->>\r\n{2}\r\n";

		public XmppSender(XmppGateway gateway)
		{
			if (gateway == null) throw new ArgumentNullException("gateway");

			this.gateway = gateway;
		}

		#region IXmppSender Members

		public void SendTo(XmppSession to, Node node)
		{
			if (to == null) throw new ArgumentNullException("to");
			SendTo(to.Stream, node);
		}

		public void SendTo(XmppStream to, Node node)
		{
			if (to == null) throw new ArgumentNullException("to");
			if (node == null) throw new ArgumentNullException("node");

			var connection = GetXmppConnection(to.ConnectionId);
			if (connection != null)
			{
				log.InfoFormat(SEND_FORMAT, to.ConnectionId, to.Namespace, node.ToString(Formatting.Indented));
				validator.ValidateNode(node, to, null);
				connection.Send(node, Encoding.UTF8);
			}
		}

		public void SendTo(XmppStream to, string text)
		{
			if (to == null) throw new ArgumentNullException("to");
			if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");

			var connection = GetXmppConnection(to.ConnectionId);
			if (connection != null)
			{
				log.InfoFormat(SEND_FORMAT, to.ConnectionId, to.Namespace, text);
				connection.Send(text, Encoding.UTF8);
			}
		}

		public void CloseStream(XmppStream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");

			var connection = GetXmppConnection(stream.ConnectionId);
			if (connection != null)
			{
				connection.Close();
			}
		}

		public void SendToAndClose(XmppStream to, Node node)
		{
			SendTo(to, node);
			SendTo(to, string.Format("</stream:{0}>", Uri.PREFIX));
			CloseStream(to);
		}

		public void ResetStream(XmppStream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");

			var connection = GetXmppConnection(stream.ConnectionId);
			if (connection != null)
			{
				connection.Reset();
			}
		}

		public IXmppConnection GetXmppConnection(string connectionId)
		{
			return gateway.GetXmppConnection(connectionId);
		}

		public bool Broadcast(ICollection<XmppSession> sessions, Node node)
		{
			if (sessions == null) throw new ArgumentNullException("sessions");
			foreach (var session in sessions)
			{
				SendTo(session, node);
			}
			return 0 < sessions.Count;
		}

		#endregion
	}
}