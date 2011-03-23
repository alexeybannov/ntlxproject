using System.Collections.Generic;
using log4net;

namespace ASC.Common.Utils
{
    public sealed class LogHolder
    {
        public static ILog Log(string loggerName)
        {
            return LogManager.GetLogger(loggerName);
        }

        public static ILog Log()
        {
            return Log("ASC");
        }
    }
}