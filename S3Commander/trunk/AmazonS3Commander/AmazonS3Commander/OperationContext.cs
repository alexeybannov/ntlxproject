using System;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    static class OperationContext
    {
        [ThreadStatic]
        private static string _remoteDir;

        [ThreadStatic]
        private static StatusOperation _operation = StatusOperation.None;


        public static string RemoteDir
        {
            get { return _remoteDir; }
        }

        public static StatusOperation Operation
        {
            get { return _operation; }
        }


        internal static void OperationBegin(string remoteDir, StatusOperation operation)
        {
            _remoteDir = remoteDir;
            _operation = operation;
        }

        internal static void OperationEnd()
        {
            _remoteDir = null;
            _operation = StatusOperation.None;
        }
    }
}
