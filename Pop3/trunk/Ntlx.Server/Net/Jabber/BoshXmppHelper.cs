﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using agsXMPP.protocol.extensions.bosh;
using ASC.Xmpp.Server.Statistics;
using log4net;

namespace ASC.Xmpp.Server.Gateway
{
	using agsXMPP.util;

	class BoshXmppHelper
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(BoshXmppHelper));

		public static bool CompressResponse
		{
			get;
			set;
		}

		public static Body ReadBodyFromRequest(HttpListenerContext ctx)
		{
			if (ctx == null) throw new ArgumentNullException("ctx");

			try
			{
				if (!ctx.Request.HasEntityBody) return null;

				byte[] data = new byte[ctx.Request.ContentLength64];
				int offset = 0;
				int count = data.Length;
				// Read may return anything from 0 to ContentLength64.
				while (0 < count)
				{
					int readed = ctx.Request.InputStream.Read(data, offset, count);
					if (readed == 0) break;
					offset += readed;
					count -= readed;
				}
				NetStatistics.ReadBytes(count);
				return ElementSerializer.DeSerializeElement<Body>(Encoding.UTF8.GetString(data));
			}
			catch (Exception e)
			{
				log.ErrorFormat("Error read body from request: {0}", e);
			}
			return null;
		}

		public static void TerminateBoshSession(HttpListenerContext ctx)
		{
			TerminateBoshSession(ctx, null, null);
		}

		public static void TerminateBoshSession(HttpListenerContext ctx, string condition)
		{
			TerminateBoshSession(ctx, null, condition);
		}

		public static void TerminateBoshSession(HttpListenerContext ctx, Body body)
		{
			TerminateBoshSession(ctx, body, null);
		}

		public static void TerminateBoshSession(HttpListenerContext ctx, Body body, string condition)
		{
			if (ctx == null || ctx.Response == null) return;

			if (body == null) body = new Body();
			try
			{
				body.Type = BoshType.terminate;
				if (!string.IsNullOrEmpty(condition)) body.SetAttribute("condition", condition);

				SendAndCloseResponse(ctx, body.ToString());

				log.DebugFormat("TerminateBoshSession body: {0}", body);
			}
			catch (Exception e)
			{
				try
				{
					ctx.Response.Close();
				}
				catch { }
				log.ErrorFormat("Error TerminateBoshSession body: {0}\r\n{1}", body, e);
			}
		}

		public static void SendAndCloseResponse(HttpListenerContext ctx, string text, bool throwIfError)
		{
			if (ctx == null) throw new ArgumentNullException("httpContext");
			var response = ctx.Response;
			try
			{
				if (string.IsNullOrEmpty(text)) return;

				response.ContentType = "text/xml; charset=utf-8";
				var buffer = Encoding.UTF8.GetBytes(text);

				if (CompressResponse)
				{
					if (Array.Exists<string>(ctx.Request.Headers.GetValues("Accept-Encoding"), v => v == "gzip"))
					{
						response.AddHeader("Content-Encoding", "gzip");
						var ms = new MemoryStream();
						var gzip = new GZipStream(ms, CompressionMode.Compress);
						gzip.Write(buffer, 0, buffer.Length);
						gzip.Close();
						buffer = ms.ToArray();
					}
				}

				response.ContentLength64 = buffer.Length;
                response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
                response.Headers.Add(HttpResponseHeader.Vary, "*");
                response.Headers.Add(HttpResponseHeader.Pragma, "no-cache");
				response.OutputStream.Write(buffer, 0, buffer.Length);
				response.OutputStream.Flush();

				NetStatistics.WriteBytes(buffer.Length);
			}
			catch
			{
				if (throwIfError) throw;
			}
			finally
			{
				try { response.Close(); }
				catch { }
			}
		}

		public static void SendAndCloseResponse(HttpListenerContext ctx, string text)
		{
			SendAndCloseResponse(ctx, text, false);
		}
	}
}