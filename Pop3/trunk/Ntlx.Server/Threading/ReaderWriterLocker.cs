using System;
using System.Threading;

namespace Ntlx.Server.Threading
{
	sealed class ReaderWriterLocker : IReaderWriterLocker
	{
		private ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);


		private ReaderWriterLocker()
		{

		}

		public static IReaderWriterLocker CreateReaderWriterLocker()
		{
			return new ReaderWriterLocker();
		}

		
		public IDisposable ReaderLock()
		{
			return new Lock(locker, true);
		}

		public IDisposable WriterLock()
		{
			return new Lock(locker, false);
		}

		
		private class Lock : IDisposable
		{
			private ReaderWriterLockSlim locker;
			private bool read;

			public Lock(ReaderWriterLockSlim locker, bool read)
			{
				this.locker = locker;
				this.read = read;
				if (read)
				{
					locker.EnterReadLock();
				}
				else
				{
					locker.EnterWriteLock();
				}
			}

			public void Dispose()
			{
				if (read)
				{
					locker.ExitReadLock();
				}
				else
				{
					locker.ExitWriteLock();
				}
			}
		}
	}
}
