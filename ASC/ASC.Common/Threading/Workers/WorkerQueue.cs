#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace ASC.Common.Threading.Workers
{
    public class WorkerQueuePersistent<T>:WorkerQueue<T>
    {
        public WorkerQueuePersistent(int workerCount, TimeSpan waitInterval) : base(workerCount, waitInterval)
        {
        }

        public WorkerQueuePersistent(int workerCount, TimeSpan waitInterval, int errorCount, bool stopAfterFinsih) : base(workerCount, waitInterval, errorCount, stopAfterFinsih)
        {
        }

        protected override WorkItem<T> Selector()
        {
            return Items.Where(x => !x.IsProcessed).Where(x => !x.IsCompleted).OrderBy(x => x.Added).FirstOrDefault();
        }

        protected override void PostComplete(WorkItem<T> item)
        {
            item.Completed = DateTime.Now;
            item.IsCompleted = true;
        }

        protected override void ErrorLimit(WorkItem<T> item)
        {
            PostComplete(item);
        }

    }

    public class WorkerQueue<T>
    {
        private readonly AutoResetEvent _emptyEvent = new AutoResetEvent(false);
        private readonly int _errorCount;
        private readonly List<WorkItem<T>> _items = new List<WorkItem<T>>();
        private readonly bool _stopAfterFinsih;

        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private readonly AutoResetEvent _waitEvent = new AutoResetEvent(false);
        private readonly int _waitInterval;
        private readonly WaitHandle[] _waitObjects = new WaitHandle[2];
        private readonly int _workerCount;
        private readonly List<Thread> _workerThreads = new List<Thread>();
        private Action<T> _action;
        private bool _isThreadSet;

        public object SynchRoot { get { return _items; } }

        public WorkerQueue(int workerCount, TimeSpan waitInterval) : this(workerCount, waitInterval, 1, false)
        {
        }

        public WorkerQueue(int workerCount, TimeSpan waitInterval, int errorCount, bool stopAfterFinsih)
        {
            _workerCount = workerCount;
            _errorCount = errorCount;
            _stopAfterFinsih = stopAfterFinsih;
            _waitInterval = (int) waitInterval.TotalMilliseconds;
            _waitObjects[0] = _stopEvent;
            _waitObjects[1] = _waitEvent;
        }

        public bool IsStarted { get; set; }

        protected internal List<WorkItem<T>> Items
        {
            get { return _items; }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (_items)
            {
                _items.AddRange(items.Select(x => new WorkItem<T>(x)));
            }
            _waitEvent.Set();
            ReviveThreads();
        }

        public virtual void Add(T item)
        {
            lock (_items)
            {
                Items.Add(new WorkItem<T>(item));
            }
            _waitEvent.Set();
            ReviveThreads();
        }

        private void ReviveThreads()
        {
            if (_workerThreads.Count != 0)
            {
                bool haveLiveThread = _workerThreads.Count(x => x.IsAlive) > 0;
                if (!haveLiveThread)
                {
                    Restart();
                }
            }
        }

        private void Restart()
        {
            Stop();
            Start(_action);
        }

        public void Remove(T item)
        {
            lock (_items)
            {
                WorkItem<T> existing = Items.Find(x => Equals(x.Item, item));
                RemoveInternal(existing);
            }
        }

        public IEnumerable<T> GetItems()
        {
            lock (_items)
            {
                return _items.Select(x => x.Item).ToList();
            }
        }

        public void Start(Action<T> starter)
        {
            lock (_items)
            {
                _action = starter;
            }
            if (!_isThreadSet)
            {
                for (int i = 0; i < _workerCount; i++)
                {
                    _workerThreads.Add(new Thread(DoWork) {Name = "queue_worker_" + (i + 1)});
                }
                _isThreadSet = true;
            }
            if (!IsStarted)
            {
                _stopEvent.Reset();
                _waitEvent.Reset();
                foreach (Thread workerThread in _workerThreads)
                {
                    workerThread.Start(_stopAfterFinsih);
                }
                IsStarted = true;
            }
        }

        public void WaitForCompletion()
        {
            _emptyEvent.WaitOne();
        }

        public void Terminate()
        {
            if (IsStarted)
            {
                _stopEvent.Set();
                _waitEvent.Set();

                foreach (Thread workerThread in _workerThreads)
                {
                    workerThread.Abort();
                }
                IsStarted = false;
                _isThreadSet = false;
                _workerThreads.Clear();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                _stopEvent.Set();
                _waitEvent.Set();

                foreach (Thread workerThread in _workerThreads)
                {
                    workerThread.Join();
                }
                IsStarted = false;
                _isThreadSet = false;
                _workerThreads.Clear();
            }
        }

        protected virtual WorkItem<T> Selector()
        {
            return Items.Where(x => !x.IsProcessed).OrderBy(x => x.Added).FirstOrDefault();
        }

        protected virtual void PostComplete(WorkItem<T> item)
        {
            item.Completed = DateTime.Now;
            RemoveInternal(item);
        }

        protected void RemoveInternal(WorkItem<T> item)
        {
            if (item != null)
            {
                _items.Remove(item);
                item.Dispose();
            }
        }

        protected virtual void ErrorLimit(WorkItem<T> item)
        {
            RemoveInternal(item);
        }

        protected virtual void Error(WorkItem<T> item, Exception exception)
        {
            item.Error = exception;
            item.IsProcessed = false;
            item.Added = DateTime.Now;
        }

        private void DoWork(object state)
        {
            bool stopAfterFinsih = false;
            if (state != null && state is bool)
            {
                stopAfterFinsih = (bool) state;
            }
            do
            {
                WorkItem<T> item;
                Action<T> localAction;
                lock (_items)
                {
                    localAction = _action;
                    item = Selector();
                    if (item != null)
                    {
                        item.IsProcessed = true;
                    }
                }

                if (item != null)
                {
                    try
                    {
                        localAction(item.Item);
                        bool fallSleep = false;
                        lock (Items)
                        {
                            PostComplete(item);
                            if (_items.Count == 0)
                            {
                                _emptyEvent.Set();
                                fallSleep = QueueEmpty(true);
                            }
                        }
                        if (fallSleep)
                        {
                            if (WaitHandle.WaitAny(_waitObjects, Timeout.Infinite, false) == 0)
                            {
                                break;
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                    }
                    catch (Exception e)
                    {
                        lock (_items)
                        {
                            Error(item, e);
                            item.ErrorCount++;
                            if (item.ErrorCount > _errorCount)
                            {
                                ErrorLimit(item);
                            }
                        }
                    }
                }
                else
                {
                    if (WaitHandle.WaitAny(_waitObjects, GetSleepInterval(), false) == 0 || stopAfterFinsih)
                    {
                        break;
                    }
                }
            } while (true);
        }

        protected virtual bool QueueEmpty(bool fallAsleep)
        {
            return fallAsleep;
        }

        protected virtual int GetSleepInterval()
        {
            return _waitInterval;
        }

        public void Clear()
        {
            lock (_items)
            {
                _items.ForEach(x => x.Dispose());
                _items.Clear();
            }
        }
    }
}