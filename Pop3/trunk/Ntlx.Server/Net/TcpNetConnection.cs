using System;
using Ntlx.Server.Utils;
using System.Net.Sockets;
using System.Net;

namespace Ntlx.Server.Net
{
	public class TcpNetConnection : ConnectionBase
	{
		private NetworkStream netStream;

		private byte[] buffer;

		private EndPoint endPoint;


		public TcpNetConnection(Socket socket)
			: base(UniqueId.Generate())
		{
			netStream = new NetworkStream(socket, true);
			buffer = new byte[socket.ReceiveBufferSize];
			endPoint = socket.RemoteEndPoint;
		}


		public override void BeginReceive()
		{
			try
			{
				if (netStream != null && netStream.CanRead)
				{
					netStream.BeginRead(buffer, 0, buffer.Length, BeginReadCallback, netStream);
				}
			}
			catch (ObjectDisposedException) { }
		}

		private void BeginReadCallback(IAsyncResult asyncResult)
		{
			try
			{
				var stream = (NetworkStream)asyncResult.AsyncState;
				var readed = stream.EndRead(asyncResult);
				if (0 < readed)
				{
					RaiseReceived(buffer, 0, readed);
					BeginReceive();
				}
				else
				{
					Close(ConnectionCloseReason.ClientShutdown);
				}
			}
			catch (Exception)
			{
				Close(ConnectionCloseReason.Error);
			}
		}


		public override void Send(byte[] buffer, int offset, int count)
		{
			try
			{
				netStream.BeginWrite(buffer, 0, count, BeginWriteCallback, netStream);
			}
			catch (Exception)
			{
				Close(ConnectionCloseReason.Error);
			}
		}

		private void BeginWriteCallback(IAsyncResult asyncResult)
		{
			var stream = (NetworkStream)asyncResult.AsyncState;
			try
			{
				stream.EndWrite(asyncResult);
			}
			catch (Exception)
			{
				Close(ConnectionCloseReason.Error);
			}
		}

		public override void OnClose(ConnectionCloseReason reason)
		{
			try
			{
				netStream.Close();
				netStream = null;
			}
			catch (Exception)
			{
			}
		}
	}
}
