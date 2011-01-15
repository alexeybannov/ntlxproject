#region usings

using System;
using System.Diagnostics;
using log4net.Core;

#endregion

namespace ASC.Common.Utils
{
    public static class DebugUtil
    {
        public static IDisposable Watch(string text)
        {
            return new WatchDispWrapper(text);
        }

        #region Nested type: WatchDispWrapper

        private class WatchDispWrapper : IDisposable
        {
            private readonly Stopwatch stopwatch;
            private readonly string text;

            public WatchDispWrapper(string text)
            {
                this.text = text;
                stopwatch = Stopwatch.StartNew();
            }

            #region IDisposable Members

            public void Dispose()
            {
                stopwatch.Stop();
                string msg = String.Format("Action: {0} complited by {1}", text, stopwatch.Elapsed);
                Debug.WriteLine(msg);
                LogHolder.Log("ASC.Debug.Profiler")
                    .Logger.Log(null, Level.Trace, msg, null);
            }

            #endregion
        }

        #endregion
    }
}