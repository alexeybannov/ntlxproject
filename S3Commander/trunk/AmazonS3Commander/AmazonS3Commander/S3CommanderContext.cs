using System;
using AmazonS3Commander.Logger;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    class S3CommanderContext
    {
        [ThreadStatic]
        private static string currentRemoteDir;

        [ThreadStatic]
        private static StatusOperation currentOperation = StatusOperation.None;

        private TotalCommanderWfxPlugin plugin;


        public string PluginName
        {
            get { return plugin.PluginName; }
        }

        public Progress Progress
        {
            get { return plugin.Progress; }
        }

        public ILog Log
        {
            get;
            private set;
        }

        public Request Request
        {
            get { return plugin.Request; }
        }

        public string CurrentDirectory
        {
            get { return currentRemoteDir; }
        }

        public StatusOperation CurrentOperation
        {
            get { return currentOperation; }
        }


        public S3CommanderContext(TotalCommanderWfxPlugin plugin, ILog log)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");
            if (log == null) throw new ArgumentNullException("log");
            
            this.plugin = plugin;
            this.Log = log;
        }

        public static void ProcessOperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            currentRemoteDir = origin == StatusOrigin.Start ? remoteDir : null;
            currentOperation = origin == StatusOrigin.Start ? operation : StatusOperation.None;
        }
    }
}
