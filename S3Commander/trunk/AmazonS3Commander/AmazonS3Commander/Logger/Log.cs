using System;
using System.Diagnostics;

namespace AmazonS3Commander.Logger
{
    class Log : ILog
    {
        public void Error(string format, params object[] args)
        {
            //Debug.Fail(string.Format(format, args));
            System.Diagnostics.Trace.TraceError(format, args);
        }

        public void Error(Exception error)
        {
            Error("{0}", error);
        }

        public void Trace(string format, params object[] args)
        {
            //Debug.WriteLine(string.Format(format, args));
            System.Diagnostics.Trace.TraceInformation(format, args);
        }
    }
}
