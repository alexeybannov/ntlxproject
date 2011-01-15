using System;
using System.Linq;
using ASC.Common.Threading.Workers;

namespace ASC.Common.Threading.Progress
{
    public class ProgressQueue : WorkerQueuePersistent<IProgressItem>
    {
        private readonly bool _removeAfterCompleted;

        public ProgressQueue(int workerCount, TimeSpan waitInterval) : this(workerCount, waitInterval, true)
        {
        }

        public ProgressQueue(int workerCount, TimeSpan waitInterval, bool removeAfterCompleted) : base(workerCount, waitInterval,0,false)
        {
            _removeAfterCompleted = removeAfterCompleted;
            Start(x=>x.RunJob());
        }

        public override void Add(IProgressItem item)
        {
            //check exists
            if (GetStatus(item.Id) == null)
            {
                base.Add(item);
            }
        }

        public IProgressItem GetStatus(object id)
        {
            IProgressItem item;
            lock (SynchRoot)
            {
                item = GetItems().Where(x => Equals(x.Id, id)).SingleOrDefault();
                if (item!=null)
                {
                    if (_removeAfterCompleted && item.IsCompleted)
                    {
                        Remove(item);
                    }
                    return item.Clone() as IProgressItem;
                }
            }
            return item;
        }
    }
}