using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using log4net;

namespace ASC.Xmpp.Server.Gateway
{
	class TcpXmppListener : XmppListenerBase
	{
		private IPEndPoint bindEndPoint = new IPEndPoint(IPAddress.Any, 5222);
		private X509Certificate certificate;
		private TcpListener tcpListener;

		private static readonly ILog log = LogManager.GetLogger(typeof(TcpXmppListener));

		public override void Configure(IDictionary<string, string> properties)
		{
			try
			{
				if (properties.ContainsKey("bindPort"))
				{
					bindEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(properties["bindPort"]));
				}
				if (properties.ContainsKey("certificate"))
				{
					if (File.Exists(properties["certificate"]))
					{
						try
						{
							certificate = X509Certificate.CreateFromCertFile(properties["certificate"]);
						}
						catch { }
					}
				}
				XmppRuntimeInfo.Port = bindEndPoint.Port;
				log.DebugFormat("Configure listener '{0}' on {1}", Name, bindEndPoint);
			}
			catch (Exception e)
			{
				log.ErrorFormat("Error configure listener '{0}': {1}", Name, e);
				throw;
			}
		}

		protected override void DoStart()
		{
			tcpListener = new TcpListener(bindEndPoint);
			tcpListener.Start();
			tcpListener.BeginAcceptSocket(BeginAcceptCallback, tcpListener);
		}

		protected override void DoStop()
		{
			tcpListener.Stop();
			tcpListener = null;
		}

		private void BeginAcceptCallback(IAsyncResult asyncResult)
		{
			try
			{
				if (!Started) return;

				var tcpListener = (TcpListener)asyncResult.AsyncState;

				tcpListener.BeginAcceptSocket(BeginAcceptCallback, tcpListener);

				var socket = tcpListener.EndAcceptSocket(asyncResult);
				AddNewXmppConnection(certificate == null ? new TcpXmppConnection(socket) : new TcpSslXmppConnection(socket, certificate));
			}
			catch (ObjectDisposedException) { return; }
			catch (Exception e)
			{
				log.ErrorFormat("Error listener '{0}' on AcceptCallback: {1}", Name, e);
			}
		}
	}
}
