using System;
using System.Collections;
using System.Collections.Generic;
using Ntlx.Server.Threading;

namespace Ntlx.Server.Collections.Generic
{
	public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private IReaderWriterLocker locker = ReaderWriterLocker.CreateReaderWriterLocker();

		private IDictionary<TKey, TValue> dic;


		public ThreadSafeDictionary()
		{
			dic = new Dictionary<TKey, TValue>();
		}

		public ThreadSafeDictionary(int capacity)
		{
			dic = new Dictionary<TKey, TValue>(capacity);
		}


		public int Count
		{
			get { using (locker.ReaderLock()) return dic.Count; }
		}

		public bool IsReadOnly
		{
			get { return dic.IsReadOnly; }
		}

		public ICollection<TKey> Keys
		{
			get { using (locker.ReaderLock()) return dic.Keys; }
		}

		public ICollection<TValue> Values
		{
			get { using (locker.ReaderLock()) return dic.Values; }
		}

		public TValue this[TKey key]
		{
			get
			{
				using (locker.ReaderLock()) return dic[key];
			}
			set
			{
				using (locker.WriterLock()) dic[key] = value;
			}
		}

		public void Add(TKey key, TValue value)
		{
			using (locker.WriterLock()) dic.Add(key, value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			using (locker.WriterLock()) dic.Add(item);
		}

		public bool Remove(TKey key)
		{
			using (locker.WriterLock()) return dic.Remove(key);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			using (locker.WriterLock()) return dic.Remove(item);
		}

		public void Clear()
		{
			using (locker.WriterLock()) dic.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			using (locker.ReaderLock()) return dic.Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			using (locker.ReaderLock()) return dic.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			using (locker.ReaderLock()) return dic.TryGetValue(key, out value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			using (locker.ReaderLock()) return dic.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			using (locker.ReaderLock()) return ((IEnumerable)dic).GetEnumerator();
		}
	}
}
