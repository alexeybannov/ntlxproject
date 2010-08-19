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


        internal static void ProcessOperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            _remoteDir = origin == StatusOrigin.Start ? remoteDir : null;
            _operation = origin == StatusOrigin.Start ? operation : StatusOperation.None;
        }
    }
}
