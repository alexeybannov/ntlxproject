using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntlx.Server.Net;
using Ntlx.Server.Sessions;
using Ntlx.Server.Formatters;
using Ntlx.Server.Collections.Generic;

namespace Ntlx.Server.Dispatcher
{
	public class Dispatcher
	{
		private Gateway gateway;

		private SessionManager sessionManager = new SessionManager();

		private IDictionary<string, IFormatter> formatters = new ThreadSafeDictionary<string, IFormatter>();


		public Dispatcher(Gateway gateway)
		{
			this.gateway = gateway;
			gateway.OpenConnection += OpenConnection;
			gateway.CloseConnection += CloseConnection;
		}


		private void OpenConnection(object sender, ConnectionOpenEventArgs e)
		{
			sessionManager.AddSession(new Session(e.Connection.Id));
			e.Connection.Received += ReceivedData;
			e.Formatter.DeserializedMessage += DeserializedMessage;
			formatters[e.Connection.Id] = e.Formatter;
		}

		private void CloseConnection(object sender, ConnectionCloseEventArgs e)
		{
			e.Connection.Received -= ReceivedData;
			formatters[e.Connection.Id].DeserializedMessage -= DeserializedMessage;
			formatters.Remove(e.Connection.Id);
			sessionManager.RemoveSession(e.Connection.Id);
		}

		private void ReceivedData(object sender, ConnectionBufferEventArgs e)
		{
			formatters[e.Connection.Id].WriteMessage(e.Buffer, e.Offset, e.Count);
		}

		private void DeserializedMessage(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
