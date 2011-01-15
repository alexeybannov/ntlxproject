#region usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace ASC.Collections
{
    internal class SortedQueue<T> : IEnumerable<T>, ICollection
    {
        internal readonly Comparison<T> _comparison;
        internal readonly List<T> _list = new List<T>();
        protected object _syncRoot = new object();

        public SortedQueue()
            : this((i1, i2) => Comparer.Default.Compare(i1, i2))
        {
        }

        public SortedQueue(Comparison<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparison = comparer;
        }

        #region ISortedQueue<T>

        public virtual void Enqueue(T item)
        {
            if (!(item is ValueType) && Equals(item, default(T)))
                throw new ArgumentNullException("item");
            lock (_syncRoot)
            {
                _list.Add(item);
                _list.Sort(_comparison);
            }
        }

        public virtual void Enqueue(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            lock (_syncRoot)
            {
                _list.AddRange(items);
                _list.Sort(_comparison);
            }
        }

        public virtual T Dequeue()
        {
            lock (_syncRoot)
            {
                T value = _list[0];
                _list.RemoveAt(0);
                return value;
            }
        }

        public virtual bool Contains(T item)
        {
            lock (_syncRoot)
            {
                return _list.Contains(item);
            }
        }

        public virtual T Peek()
        {
            lock (_syncRoot)
            {
                return _list[0];
            }
        }

        public virtual bool Remove(T item)
        {
            lock (_syncRoot)
            {
                return _list.RemoveAll(element => element.Equals(item)) > 0;
            }
        }

        #endregion

        #region ICollection

        public void CopyTo(Array array, int index)
        {
            lock (_syncRoot)
                ((ICollection) _list).CopyTo(array, index);
        }

        public int Count
        {
            get { lock (_syncRoot) return _list.Count; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}