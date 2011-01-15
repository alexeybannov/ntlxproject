using System;
using System.Diagnostics;
using log4net;

namespace ASC.Common.Utils
{
    public class TimeProfiler:IDisposable
    {
        private readonly string _name;
        private readonly ILog _log;
        private readonly Stopwatch _sw;

        public TimeProfiler(string name, ILog log)
        {
            if (log == null) throw new ArgumentNullException("log");
            _name = name;
            _log = log;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public long AddCheckpoint(string name)
        {
            _log.DebugFormat("{0}:added checkpoint {1}:{2}",_name, name,_sw.Elapsed);
            return _sw.ElapsedMilliseconds;
        }

        public long Checkpoint(string name)
        {
            long time = _sw.ElapsedMilliseconds;
            _log.DebugFormat("{0}:checkpoint {1}:{2}", _name, name, _sw.Elapsed);
            _sw.Reset();
            _sw.Start();
            return time;
        }

        public void Dispose()
        {
            _log.DebugFormat("{0}:counter ended {1}",_name, _sw.Elapsed);
        }
    }
}