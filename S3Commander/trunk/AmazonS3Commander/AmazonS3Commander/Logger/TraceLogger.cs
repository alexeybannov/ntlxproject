using System;
using System.Diagnostics;

namespace AmazonS3Commander.Logger
{
    class TraceLogger : ILog
    {
        public TraceLogger(string logfile)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(logfile));
            Trace.AutoFlush = true;
        }


        public void Error(string format, params object[] args)
        {
            AddDateTime();
            Trace.TraceError(format, args);
        }

        public void Error(Exception error)
        {
            AddDateTime();
            Trace.TraceError(error.ToString());
        }


        public void Info(string message)
        {
            AddDateTime();
            Trace.TraceInformation(message);
        }

        public void Info(string format, params object[] args)
        {
            AddDateTime();
            Trace.TraceInformation(format, args);
        }

        private void AddDateTime()
        {
            Trace.Write(DateTime.Now.ToString("HH:mm:ss.fff") + ": ");
        }
    }
}
