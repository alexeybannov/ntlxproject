using System;
using System.Diagnostics;
using System.IO;
using AmazonS3Commander.Properties;
using SystemTrace = System.Diagnostics.Trace;

namespace AmazonS3Commander.Logger
{
    class Log : ILog
    {
        public Log(string path)
        {
            var logfile = Path.Combine(path, Resources.ProductName + ".log");
            SystemTrace.Listeners.Add(new TextWriterTraceListener(logfile));
            SystemTrace.AutoFlush = true;
            Trace("\r\n\r\n\r\n{1} *** Start {0} plugin\r\n", Resources.ProductName, DateTime.UtcNow);
        }


        public void Error(string format, params object[] args)
        {
            SystemTrace.TraceError(format, args);
        }

        public void Error(Exception error)
        {
            Error("{0}", error);
        }


        public void Trace(string message)
        {
            SystemTrace.TraceInformation(message);
        }

        public void Trace(string format, params object[] args)
        {
            SystemTrace.TraceInformation(format, args);
        }
    }
}
