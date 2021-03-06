﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AmazonS3Commander.Logger;
using AmazonS3Commander.Resources;
using LitS3;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class S3CommanderPlugin : TotalCommanderWfxPlugin
    {
        private S3CommanderContext context;

        private S3CommanderRoot root;

        private ILog log;


        public override string PluginName
        {
            get { return RS.ProductName; }
        }

        public override BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.AskUser; }
        }


        public override void Initialize()
        {
            var workDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PluginName);
            if (!Directory.Exists(workDirectory))
            {
                Directory.CreateDirectory(workDirectory);
            }

            log = new TraceLogger(Path.Combine(workDirectory, PluginName + ".log"));
            context = new S3CommanderContext(this, log);
            root = new S3CommanderRoot(workDirectory);
            root.Initialize(context);

            Trace("\r\n\r\n\r\n{1} *** Start {0} plugin\r\n", PluginName, DateTime.Now);
        }


        public override FindData FindFirst(string path, out IEnumerator enumerator)
        {
            Trace("FindFirst(path = \"{0}\")", path);
            enumerator = ResolvePath(path).GetFiles();
            return FindNext(enumerator);
        }

        public override FindData FindNext(IEnumerator enumerator)
        {
            if (enumerator == null) return FindData.NotOpen;
            return enumerator.MoveNext() ? (FindData)enumerator.Current : FindData.NoMoreFiles;
        }


        public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
        {
            Trace("ExecuteOpen(path = \"{0}\")", remoteName);
            return ResolvePath(remoteName).Open(window, ref remoteName);
        }

        public override ExecuteResult ExecuteProperties(TotalCommanderWindow window, ref string remoteName)
        {
            Trace("ExecuteProperties(path = \"{0}\")", remoteName);
            return ResolvePath(remoteName).Properties(window, ref remoteName);
        }


        public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
        {
            Trace("FileCopy(from = \"{0}\", to = \"{1}\", overwrite = {2}, move = {3})", source, target, overwrite, move);
            return ResolvePath(source).CopyTo(ResolvePath(target), overwrite, move, ri);
        }

        public override FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            Trace("FileGet(from = \"{0}\", to = \"{1}\", flags = {2})", remoteName, localName, copyFlags);
            return ResolvePath(remoteName).Download(localName, copyFlags, ri);
        }

        public override FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            Trace("FilePut(from = \"{0}\", to = \"{1}\", flags = {2})", localName, remoteName, copyFlags);
            return ResolvePath(remoteName).Upload(localName, copyFlags);
        }

        public override bool FileRemove(string remoteName)
        {
            Trace("FileRemove(path = \"{0}\")", remoteName);
            return ResolvePath(remoteName).DeleteFile();
        }


        public override bool DirectoryCreate(string path)
        {
            Trace("DirectoryCreate(path = \"{0}\")", path);
            return ResolvePath(path).CreateFolder();
        }

        public override bool DirectoryRemove(string remoteName)
        {
            Trace("DirectoryRemove(path = \"{0}\")", remoteName);
            return ResolvePath(remoteName).DeleteFolder();
        }


        public override CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlags extractIconFlag, out Icon icon)
        {
            icon = ResolvePath(remoteName).GetIcon();
            return icon != null ? CustomIconResult.Extracted : CustomIconResult.UseDefault;
        }

        public override void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {
            Trace("Command '{0}' for '{1}' {2}", operation, remoteName, origin.ToString().ToLower());
            S3CommanderContext.ProcessOperationInfo(remoteName, origin, operation);
        }

        public override void OnError(Exception error)
        {
            log.Error(error);
            if (error is S3Exception)
            {
                var s3error = (S3Exception)error;
                MessageBox.Show(error.Message, PluginName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(error.ToString(), PluginName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private S3CommanderFile ResolvePath(string path)
        {
            return root.ResolvePath(path) ?? S3CommanderFile.Empty;
        }

        [Conditional("DEBUG")]
        private void Trace(string format, params object[] args)
        {
            log.Info(format, args);
        }
    }
}
