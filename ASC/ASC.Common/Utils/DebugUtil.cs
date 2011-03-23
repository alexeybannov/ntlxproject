using System;
using System.Diagnostics;
using log4net.Core;

namespace ASC.Common.Utils
{
    public class DebugUtil : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly string text;


        public static IDisposable Watch(string text)
        {
            return new DebugUtil(text);
        }

        public DebugUtil(string text)
        {
            this.text = text;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            var msg = String.Format("Action: {0} complited by {1}", text, stopwatch.Elapsed);
            Debug.WriteLine(msg);
            LogHolder.Log("ASC.Debug.Profiler").Logger.Log(null, Level.Trace, msg, null);
        }
    }
}