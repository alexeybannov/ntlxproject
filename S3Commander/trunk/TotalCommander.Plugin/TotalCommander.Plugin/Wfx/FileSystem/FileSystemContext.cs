using System;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public sealed class FileSystemContext
    {
        internal FileSystemContext()
        {

        }


        public string PluginName
        {
            get;
            internal set;
        }

        public int PluginNumber
        {
            get;
            internal set;
        }

        public Progress Progress
        {
            get;
            internal set;
        }

        public Log Log
        {
            get;
            internal set;
        }

        public Request Request
        {
            get;
            internal set;
        }

        public Password Password
        {
            get;
            internal set;
        }

        public Version PluginInterfaceVersion
        {
            get;
            internal set;
        }

        public string IniFilePath
        {
            get;
            internal set;
        }

        public bool TemporaryPanelPlugin
        {
            get;
            internal set;
        }

        public BackgroundFlags BackgroundSupport
        {
            get;
            internal set;
        }
    }
}
