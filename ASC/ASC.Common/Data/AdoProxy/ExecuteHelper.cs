using System;
using System.Diagnostics;

namespace ASC.Common.Data.AdoProxy
{
    class ExecuteHelper : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly Action<TimeSpan> onStop;


        private ExecuteHelper(Action<TimeSpan> onStop)
        {
            if (onStop == null) throw new ArgumentNullException("onStop");

            this.onStop = onStop;
            stopwatch = Stopwatch.StartNew();
        }


        public void Dispose()
        {
            stopwatch.Stop();
            try
            {
                onStop(stopwatch.Elapsed);
            }
            catch { }
        }


        public static IDisposable Begin(Action<TimeSpan> onStop)
        {
            return new ExecuteHelper(onStop);
        }
    }
}
