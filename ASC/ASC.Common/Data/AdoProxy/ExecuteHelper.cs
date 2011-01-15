#region usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    internal class ExecuteHelper
        : IDisposable
    {
        private Stopwatch _sw;

        public static ExecuteHelper Begin(Action<TimeSpan?> onStop, bool needProfiling)
        {
            var ph = new ExecuteHelper();
            if (onStop != null)
                ph.StopEvent += onStop;
            if (needProfiling)
            {
                ph.InitProfiling();
                ph.Start();
            }
            return ph;
        }

        private void InitProfiling()
        {
            if (_sw == null)
                _sw = new Stopwatch();
        }

        public void Start()
        {
            if (_sw != null)
            {
                _sw.Reset();
                _sw.Start();
            }
        }

        public void Stop()
        {
            TimeSpan? duration = null;
            if (_sw != null)
            {
                _sw.Stop();
                duration = _sw.Elapsed;
            }
            if (StopEvent != null)
            {
                foreach (Delegate method in StopEvent.GetInvocationList())
                {
                    try
                    {
                        method.DynamicInvoke(duration);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public event Action<TimeSpan?> StopEvent;

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            if (!disposed)
            {
                if (_sw != null)
                {
                    if (_sw.IsRunning)
                        Stop();
                }
                disposed = true;
            }
        }

        ~ExecuteHelper()
        {
        }

        #endregion
    }
}