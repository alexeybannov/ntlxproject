#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace ASC.Common.Threading.Workers
{
    public class WorkerQueueReccurence<T> : WorkerQueue<T>
    {
        private readonly TimeSpan _waitInterval;

        public WorkerQueueReccurence(int workerCount, TimeSpan waitInterval) : base(workerCount, waitInterval)
        {
            _waitInterval = waitInterval;
        }

        protected override void Error(WorkItem<T> item, Exception exception)
        {
            base.Error(item, exception);
        }

        protected override void PostComplete(WorkItem<T> item)
        {
            item.Added = DateTime.Now;
            item.Completed = DateTime.Now;
            item.IsProcessed = false;
        }

        protected override int GetSleepInterval()
        {
            IEnumerable<WorkItem<T>> sleep =
                Items.Where(x => !x.IsProcessed).Where(x => (DateTime.Now - x.Completed) > _waitInterval);
            TimeSpan sleepCount = sleep.Count() > 0
                                      ? sleep.Min(x => (DateTime.Now - x.Completed))
                                      : _waitInterval;
            Debug.Print("Sleeping for {0}", sleepCount);
            return (int) sleepCount.TotalMilliseconds;
        }

        protected override WorkItem<T> Selector()
        {
            IOrderedEnumerable<WorkItem<T>> items =
                Items.Where(x => !x.IsProcessed).Where(x => (DateTime.Now - x.Completed) > _waitInterval).OrderBy(
                    x => x.Added);
            return items.FirstOrDefault();
        }
    }
}