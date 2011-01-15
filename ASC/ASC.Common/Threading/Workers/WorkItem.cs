#region usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Common.Threading.Workers
{
    [DebuggerDisplay("Processed={IsProcessed} Added={Added}")]
    public class WorkItem<T> : IDisposable
    {
        private bool _disposed;

        public WorkItem(T item)
        {
            Item = item;
            Added = DateTime.Now;
            Completed = DateTime.MinValue;
            IsProcessed = false;
        }

        public T Item { get; set; }
        internal DateTime Added { get; set; }
        internal DateTime Completed { get; set; }
        internal Exception Error { get; set; }
        public bool IsCompleted { get; set; }
        internal int ErrorCount { get; set; }
        internal bool IsProcessed { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (WorkItem<T>) && Equals((WorkItem<T>) obj);
        }

        public bool Equals(WorkItem<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(other.Item, Item);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        ~WorkItem()
        {
            Dispose(false);
        }

        public void Dispose(bool isdisposing)
        {
            if (!_disposed)
            {
                if (isdisposing)
                {
                    if (Item is IDisposable)
                    {
                        ((IDisposable) Item).Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}