using System.IO;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IStreamTransformer
	{
		Stream TransformInputStream(Stream inputStream);

		Stream TransformOutputStream(Stream outputStream);
	}
}
