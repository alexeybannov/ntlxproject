using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ASC.Xmpp.Server.Gateway
{
	class TcpSslXmppConnection : TcpXmppConnection
	{
		public TcpSslXmppConnection(Socket socket, X509Certificate cert)
			: base(socket)
		{
			sendStream = recieveStream = new SslStream(recieveStream, false);
			((SslStream)recieveStream).AuthenticateAsServer(cert, false, SslProtocols.Ssl3, false);
		}
	}
}