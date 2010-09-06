using System;

namespace AmazonS3Commander.Logger
{
    interface ILog
    {
        void Error(Exception error);

        void Error(string format, params object[] args);


        void Info(string message);

        void Info(string format, params object[] args);
    }
}
