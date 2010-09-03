using System;
using AmazonS3Commander.S3;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wfx.FileSystem;
using AmazonS3Commander.Logger;

namespace AmazonS3Commander
{
    class S3CommanderContext
    {
        [ThreadStatic]
        private static string currentRemoteDir;

        [ThreadStatic]
        private static StatusOperation currentOperation = StatusOperation.None;

        private readonly FileSystemContext context;

        private readonly S3ServiceProvider provider;


        public string PluginName
        {
            get { return context.PluginName; }
        }

        public Progress Progress
        {
            get { return context.Progress; }
        }

        public ILog Log
        {
            get;
            private set;
        }

        public Request Request
        {
            get { return context.Request; }
        }

        public Password Password
        {
            get { return context.Password; }
        }

        public string CurrentDirectory
        {
            get { return currentRemoteDir; }
        }

        public StatusOperation CurrentOperation
        {
            get { return currentOperation; }
        }

        public string CurrentAccount
        {
            get;
            private set;
        }

        public IS3Service S3Service
        {
            get { return provider.GetS3Service(CurrentAccount); }
        }


        public S3CommanderContext(FileSystemContext context, ILog log)
            : this(context, log, null, null)
        {

        }

        public S3CommanderContext(FileSystemContext context, ILog log, S3ServiceProvider provider, string accountName)
        {
            this.context = context;
            this.Log = log;
            this.provider = provider;
            CurrentAccount = accountName;
        }


        public static void ProcessOperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation)
        {
            currentRemoteDir = origin == StatusOrigin.Start ? remoteDir : null;
            currentOperation = origin == StatusOrigin.Start ? operation : StatusOperation.None;
        }
    }
}
