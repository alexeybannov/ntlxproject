#region usings

using System;
using System.Threading;

#endregion

namespace ASC.Threading
{
    public class MultiAttemptTaskQueue : TaskQueue
    {
        private readonly int AttemptCount = 5;
        private readonly TimeSpan AttemptInterval = TimeSpan.FromSeconds(1);

        public MultiAttemptTaskQueue()
        {
        }

        public MultiAttemptTaskQueue(int workerCount, int attemptCount, TimeSpan attemptInterval)
            : base(workerCount)
        {
            AttemptCount = attemptCount;
            AttemptInterval = attemptInterval;
        }

        internal override void ExecuteTask(TaskItem task)
        {
            var request = task.SystemData as TaskRequest;
            if (request == null) throw new ApplicationException("invalid task");
            request.CurrentAttempt++;
            request.LastAttempt = DateTime.UtcNow;
            bool done = false;
            try
            {
                done = task.Task(task.UserData);
            }
            catch
            {
            }
            if (!done)
            {
                if (request.CurrentAttempt < AttemptCount)
                    EnqueueTaskInternal(task);
            }
        }

        internal override void EnqueueTaskInternal(TaskItem task)
        {
            if (!(task.SystemData is TaskRequest))
                task.SystemData = new TaskRequest {Task = task};
            base.EnqueueTaskInternal(task);
        }

        internal override TaskItem GetNextTask()
        {
            lock (syncRoot)
            {
                while (
                    taskQ.Count == 0 ||
                    !(IsTaskReadyToExecute(taskQ.Peek()))
                    )
                {
                    int timeout = Timeout.Infinite;
                    if (taskQ.Count > 0)
                    {
                        timeout = (int) NeedWaitTask(taskQ.Peek()).TotalMilliseconds;
                        if (timeout <= 0) break;
                    }
                    Monitor.Wait(syncRoot, timeout);
                }
                return taskQ.Dequeue();
            }
        }

        internal TimeSpan NeedWaitTask(TaskItem task)
        {
            TimeSpan timespan = TimeSpan.Zero;
            if (task == null) return timespan;
            var req = task.SystemData as TaskRequest;
            if (req == null) return timespan;
            return (req.LastAttempt + AttemptInterval) - DateTime.UtcNow;
        }

        internal bool IsTaskReadyToExecute(TaskItem task)
        {
            if (task == null) return false;
            var req = task.SystemData as TaskRequest;
            if (req == null) return true;
            return (req.LastAttempt + AttemptInterval <= DateTime.UtcNow);
        }

        internal override int TaskItemComparision(TaskItem task1, TaskItem task2)
        {
            var request1 = task1.SystemData as TaskRequest;
            var request2 = task2.SystemData as TaskRequest;
            if (request1 == null || request2 == null) return 0;
            return request1.CompareTo(request2);
        }

        #region Nested type: TaskRequest

        internal class TaskRequest
            : IComparable
        {
            public int CurrentAttempt;
            public DateTime LastAttempt = DateTime.MinValue;
            public TaskItem Task;

            #region IComparable

            public int CompareTo(object obj)
            {
                var req = obj as TaskRequest;
                if (req == null) return 1;
                return DateTime.Compare(LastAttempt, req.LastAttempt);
            }

            #endregion
        }

        #endregion
    }
}