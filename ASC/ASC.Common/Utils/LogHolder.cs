#region usings

using System.Collections.Generic;
using log4net;

#endregion

namespace ASC.Common.Utils
{
    public sealed class LogHolder
    {
        private const string _DefaultLoggerName = "ASC";
        private static readonly IDictionary<string, ILog> _Loggers = new Dictionary<string, ILog>(2);

        public static ILog Log(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName))
                loggerName = _DefaultLoggerName;
            ILog logger = null;
            if (!_Loggers.TryGetValue(loggerName, out logger))
            {
                lock (_Loggers)
                {
                    if (!_Loggers.ContainsKey(loggerName))
                        logger = LogManager.GetLogger(loggerName);
                    else
                        logger = _Loggers[loggerName];
                }
            }
            return logger;
        }

        public static ILog Log()
        {
            return Log(_DefaultLoggerName);
        }
    }
}