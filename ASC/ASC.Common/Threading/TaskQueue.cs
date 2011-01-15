#region usings

using System;
using System.Threading;
using ASC.Collections;

#endregion

namespace ASC.Threading
{
    public delegate bool TaskExecutor(object userData);

    public class TaskQueue : IDisposable
    {
        internal readonly TaskItem StopTask = new TaskItem(o => true, null);
        private readonly Thread[] workers;
        protected object syncRoot = new object();
        internal SortedQueue<TaskItem> taskQ;

        #region

        public TaskQueue()
            : this(1)
        {
        }

        public TaskQueue(int workerCount)
        {
            taskQ = new SortedQueue<TaskItem>(TaskItemComparision);
            if (workerCount <= 0)
                throw new ArgumentException("workerCount mast greater than zero", "workerCount");
            workers = new Thread[workerCount];
            for (int i = 0; i < workerCount; i++)
                (workers[i] = new Thread(Consume)).Start();
        }

        #endregion

        public bool IsEmpty
        {
            get { lock (syncRoot) return taskQ.Count == 0; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion

        public void EnqueueTask(TaskExecutor task, object data)
        {
            if (task == null) throw new ArgumentNullException("task");
            EnqueueTaskInternal(new TaskItem(task, data));
        }

        public void Stop()
        {
            foreach (Thread worker in workers)
                EnqueueTaskInternal(StopTask);
            foreach (Thread worker in workers)
                worker.Join();
        }

        internal virtual int TaskItemComparision(TaskItem task1, TaskItem task2)
        {
            return 0;
        }

        internal virtual TaskItem GetNextTask()
        {
            lock (syncRoot)
            {
                while (taskQ.Count == 0)
                    WaitTasks();
                return taskQ.Dequeue();
            }
        }

        internal virtual void ExecuteTask(TaskItem task)
        {
            if (task == null) return;
            try
            {
                task.Task(task.UserData);
            }
            catch
            {
            }
        }

        internal virtual bool WaitTasks()
        {
            return Monitor.Wait(syncRoot);
        }

        private void Consume()
        {
            while (true)
            {
                TaskItem task = GetNextTask();
                if (ReferenceEquals(task, StopTask))
                    return;
                ExecuteTask(task);
            }
        }

        internal virtual void EnqueueTaskInternal(TaskItem task)
        {
            lock (syncRoot)
            {
                taskQ.Enqueue(task);
                Monitor.PulseAll(syncRoot);
            }
        }

        #region Nested type: TaskItem

        internal class TaskItem
        {
            public TaskItem(TaskExecutor task, object data)
            {
                if (task == null) throw new ArgumentNullException("task");
                Task = task;
                UserData = data;
            }

            public TaskExecutor Task { get; private set; }
            public object UserData { get; private set; }
            public object SystemData { get; set; }
        }

        #endregion
    }
}