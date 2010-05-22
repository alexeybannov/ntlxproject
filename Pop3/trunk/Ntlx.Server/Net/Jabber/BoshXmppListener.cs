using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using log4net;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Gateway
{
	class BoshXmppListener : XmppListenerBase
	{
		private const string DEFAULT_BIND_URL = "http://*:5280/http-poll/";
		private const string DEFAULT_POLICY_URL = "http://*:5280/";
		private const string DEFAULT_POLICY_FILE = "|DataDirectory|policy.xml";

		private readonly HttpListener httpListener = new HttpListener();

		private System.Uri bindUri;
		private System.Uri domainUri;

		private string policyFile = DEFAULT_POLICY_FILE;
		private string policy = string.Empty;
		private bool policyLoaded = false;

		private static readonly ILog log = LogManager.GetLogger(typeof(BoshXmppListener));


		public override void Configure(IDictionary<string, string> properties)
		{
			try
			{
				string hostname = Dns.GetHostName();

				string bindPrefix = properties.ContainsKey("bind") ? properties["bind"] : DEFAULT_BIND_URL;
				bindUri = new System.Uri(bindPrefix.Replace("*", hostname));
				XmppRuntimeInfo.BoshUri = bindUri;

				string policyPrefix = properties.ContainsKey("policy") ? properties["policy"] : DEFAULT_POLICY_URL;
				domainUri = new System.Uri(policyPrefix.Replace("*", hostname));

				if (policyPrefix.Contains(".")) policyPrefix = policyPrefix.Substring(0, policyPrefix.LastIndexOf("/"));
				if (!policyPrefix.EndsWith("/")) policyPrefix += "/";

                httpListener.Prefixes.Add(bindPrefix);
				httpListener.Prefixes.Add(policyPrefix);

				log.DebugFormat("Configure listener {0} on {1}", Name, bindPrefix);
				log.DebugFormat("Configure listener {0} on {1}", Name, policyPrefix);

				BoshXmppHelper.CompressResponse = true;
				if (properties.ContainsKey("compress"))
				{
					BoshXmppHelper.CompressResponse = bool.Parse(properties["compress"]);
				}

				if (properties.ContainsKey("policyFile")) policyFile = properties["policyFile"];
				policyFile = PathUtils.GetAbsolutePath(policyFile);
			}
			catch (Exception e)
			{
				log.DebugFormat("Error configure listener {0}: {1}", Name, e);
				throw;
			}
		}

		protected override void DoStart()
		{
			httpListener.Start();
			BeginGetContext();
		}

		protected override void DoStop()
		{
			httpListener.Stop();
		}

		private void BeginGetContext()
		{
			if (httpListener != null && httpListener.IsListening)
			{
				httpListener.BeginGetContext(GetContextCallback, null);
			}
		}

		private void GetContextCallback(IAsyncResult asyncResult)
		{
			HttpListenerContext ctx = null;
			try
			{
				try
				{
					ctx = httpListener.EndGetContext(asyncResult);
				}
				finally
				{
					BeginGetContext();
				}

				var url = ctx.Request.Url;
				log.DebugFormat("{0}: Begin process http request {1}", Name, url);

				if (url.AbsolutePath == bindUri.AbsolutePath)
				{
					var body = BoshXmppHelper.ReadBodyFromRequest(ctx);
					if (body == null)
					{
						BoshXmppHelper.TerminateBoshSession(ctx, "bad-request");
						return;
					}

					var connection = GetXmppConnection(body.Sid) as BoshXmppConnection;

					if (!string.IsNullOrEmpty(body.Sid) && connection == null)
					{
						BoshXmppHelper.TerminateBoshSession(ctx, "item-not-found");
						return;
					}

					if (connection == null)
					{
						connection = new BoshXmppConnection();
						AddNewXmppConnection(connection);
					}

					connection.ProcessBody(body, ctx);
				}
				else if (url.AbsolutePath == domainUri.AbsolutePath && ctx.Request.HttpMethod == "GET")
				{
					SendPolicy(ctx);
				}
				else
				{
					BoshXmppHelper.TerminateBoshSession(ctx, "bad-request");
				}
			}
			catch (ObjectDisposedException) { }
			catch (Exception e)
			{
				if (ctx != null) BoshXmppHelper.TerminateBoshSession(ctx, "internal-server-error");
				if (Started) log.ErrorFormat("{0}: Error GetContextCallback: {1}", Name, e);
			}
		}

		private void SendPolicy(HttpListenerContext ctx)
		{
			log.DebugFormat("{0}: Send policy.", Name);

			if (!policyLoaded)
			{
				try
				{
					policy = File.ReadAllText(policyFile);
				}
				catch (Exception ex)
				{
					log.ErrorFormat("Can not load policy file: {0}, error: {1}", policyFile, ex);
				}
				policyLoaded = true;
			}
			BoshXmppHelper.SendAndCloseResponse(ctx, policy);
		}
	}
}