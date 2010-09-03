using System;

namespace AmazonS3Commander.Logger
{
    interface ILog
    {
        void Error(Exception error);

        void Error(string format, params object[] args);


        void Trace(string message);

        void Trace(string format, params object[] args);
    }
}
