using System;

namespace Ntlx.Server.Threading
{
	interface IReaderWriterLocker
	{
		IDisposable ReaderLock();

		IDisposable WriterLock();
	}
}
