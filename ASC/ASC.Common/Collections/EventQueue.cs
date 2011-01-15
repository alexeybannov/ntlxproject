#region usings

using System;

#endregion

namespace ASC.Collections
{
    internal delegate DateTime? NextEventDateGather<T>(T evnt, DateTime fromDate);

    internal class EventWrapper<T>
    {
        public DateTime Date;
        public T Event;

        public EventWrapper(T evnt)
        {
            Event = evnt;
        }

        public EventWrapper(T evnt, DateTime date) : this(evnt)
        {
            Event = evnt;
            Date = date;
        }

        public override bool Equals(object obj)
        {
            if (Event == null) return false;
            var evnt = obj as EventWrapper<T>;
            if (evnt == null) return false;
            return Event.Equals(evnt.Event);
        }

        public override int GetHashCode()
        {
            if (Event == null) return 0;
            return Event.GetHashCode();
        }
    }

    internal class EventQueue<T>
    {
        private readonly NextEventDateGather<T> _gather;
        private readonly SortedQueue<EventWrapper<T>> _queue;

        public EventQueue(NextEventDateGather<T> nextDateGather)
        {
            if (nextDateGather == null) throw new ArgumentNullException("nextDateGather");
            _gather = nextDateGather;
            _queue = new SortedQueue<EventWrapper<T>>(
                (e1, e2) => DateTime.Compare(e1.Date, e2.Date)
                );
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public int ReadyCount
        {
            get { return ReadyCountImpl(DateTime.UtcNow); }
        }

        public DateTime? FirstEventDate
        {
            get
            {
                lock (_queue.SyncRoot)
                {
                    if (_queue.Count > 0)
                        return _queue.Peek().Date;
                    return null;
                }
            }
        }

        public TimeSpan? FirstEventSpan
        {
            get { return FirstEventSpanImpl(DateTime.UtcNow); }
        }

        public object SyncRoot
        {
            get { return _queue.SyncRoot; }
        }

        internal static int ComparerMethod(NextEventDateGather<T> gather, T evnt1, T evnt2)
        {
            DateTime now = DateTime.UtcNow;
            DateTime? evnt1Next = gather(evnt1, now);
            DateTime? evnt2Next = gather(evnt2, now);
            if (!evnt1Next.HasValue && !evnt2Next.HasValue) return 0;
            if (!evnt1Next.HasValue && evnt2Next.HasValue) return -1;
            if (evnt1Next.HasValue && !evnt2Next.HasValue) return +1;
            return DateTime.Compare(evnt1Next.Value, evnt2Next.Value);
        }

        internal static DateTime? GatherFromNow(NextEventDateGather<T> gather, T evnt)
        {
            return GatherFrom(gather, evnt, DateTime.UtcNow);
        }

        internal static DateTime? GatherFrom(NextEventDateGather<T> gather, T evnt, DateTime from)
        {
            if (Equals(evnt, default(T)) || gather == null) return null;
            DateTime now = from;
            DateTime? fireDate = gather(evnt, now);
            if (fireDate.HasValue && now > fireDate.Value) return null;
            return fireDate;
        }

        public void Enqueue(T evnt)
        {
            Enqueue(evnt, DateTime.UtcNow);
        }

        internal void Enqueue(T evnt, DateTime enqDate)
        {
            DateTime? fireDate = GatherFrom(_gather, evnt, enqDate);
            if (!fireDate.HasValue) return;
            _queue.Enqueue(new EventWrapper<T>(evnt, fireDate.Value));
        }

        public T Dequeue()
        {
            DateTime? fake = null;
            return Dequeue(out fake);
        }

        public T Dequeue(out DateTime? scheduleDate)
        {
            return Dequeue(out scheduleDate, DateTime.UtcNow);
        }

        internal T Dequeue(out DateTime? scheduleDate, DateTime deqDate)
        {
            scheduleDate = null;
            lock (_queue.SyncRoot)
            {
                if (_queue.Count == 0) return default(T);
                EventWrapper<T> evnt = _queue.Peek();
                if (evnt.Date <= deqDate)
                {
                    scheduleDate = evnt.Date;
                    _queue.Dequeue();
                    Enqueue(evnt.Event, deqDate);
                    return evnt.Event;
                }
                else
                    return default(T);
            }
        }

        public void Remove(T evnt)
        {
            _queue.Remove(new EventWrapper<T>(evnt));
        }

        internal int ReadyCountImpl(DateTime fromDate)
        {
            int count = 0;
            lock (_queue.SyncRoot)
            {
                foreach (var eventWrapper in _queue)
                {
                    if (eventWrapper.Date <= fromDate) count++;
                    else break;
                }
            }
            return count;
        }

        internal TimeSpan? FirstEventSpanImpl(DateTime fromDate)
        {
            DateTime? date = FirstEventDate;
            return date.HasValue ? (TimeSpan?) (date.Value - fromDate) : null;
        }
    }
}