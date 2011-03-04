using System;
using System.Diagnostics;

namespace ASC.Common.Data.AdoProxy
{
    class ExecuteHelper : IDisposable
    {
        private Stopwatch stopwatch;


        private ExecuteHelper(Action<TimeSpan> onStop)
        {
            if (onStop == null) throw new ArgumentNullException("onStop");

            StopEvent += onStop;
            stopwatch = Stopwatch.StartNew();
        }


        public event Action<TimeSpan> StopEvent;

        public void Dispose()
        {
            stopwatch.Stop();
            foreach (var method in StopEvent.GetInvocationList())
            {
                try
                {
                    method.DynamicInvoke(stopwatch.Elapsed);
                }
                catch { }
            }
        }


        public static IDisposable Begin(Action<TimeSpan> onStop)
        {
            return new ExecuteHelper(onStop);
        }
    }
}
