using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using log4net;

namespace Ntlx.Server.Net
{
	public class TcpNetListener : NetListenerBase
	{
		private IPEndPoint bindEndPoint;
		private X509Certificate certificate;
		private TcpListener tcpListener;

		private static readonly ILog log = LogManager.GetLogger(typeof(TcpNetListener));


		public override void Configure(IDictionary<string, string> properties)
		{
			try
			{
				if (!properties.ContainsKey("port")) throw new ConfigurationErrorsException("Port is not set.");
				bindEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(properties["port"]));

				if (properties.ContainsKey("cert"))
				{
					certificate = X509Certificate.CreateFromCertFile(properties["cert"]);
				}
				
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
				if (!Listening) return;

				var tcpListener = (TcpListener)asyncResult.AsyncState;

				tcpListener.BeginAcceptSocket(BeginAcceptCallback, tcpListener);

				var socket = tcpListener.EndAcceptSocket(asyncResult);
				//AddNewConnection(certificate == null ? new TcpXmppConnection(socket) : new TcpSslXmppConnection(socket, certificate));
			}
			catch (ObjectDisposedException) { }
			catch (Exception e)
			{
				log.ErrorFormat("Error listener '{0}' on AcceptCallback: {1}", Name, e);
			}
		}
	}
}
