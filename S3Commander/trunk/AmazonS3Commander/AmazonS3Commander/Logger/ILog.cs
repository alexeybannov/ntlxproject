using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonS3Commander.Logger
{
    interface ILog
    {
        void Error(Exception error);

        void Error(string format, params object[] args);

        void Trace(string format, params object[] args);
    }
}
