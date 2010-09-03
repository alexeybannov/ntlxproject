using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AmazonS3Commander.Logger;
using AmazonS3Commander.Properties;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;
using LitS3;

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
            get { return Resources.ProductName; }
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

            log = new Logger.Log(workDirectory);
            context = new S3CommanderContext(this, log);

            root = (S3CommanderRoot)new S3CommanderRoot(workDirectory)
                .Initialize(context);
        }


        public override FindData FindFirst(string path, out IEnumerator enumerator)
        {
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
            return ResolvePath(remoteName).Open(window, ref remoteName);
        }

        public override ExecuteResult ExecuteProperties(TotalCommanderWindow window, ref string remoteName)
        {
            return ResolvePath(remoteName).Properties(window, ref remoteName);
        }


        public override FileOperationResult FileCopy(string source, string target, bool overwrite, bool move, RemoteInfo ri)
        {
            if (!overwrite)
            {
                return FileOperationResult.Exists;
            }
            return ResolvePath(source).CopyTo(ResolvePath(target), move, ri);
        }

        public override FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return ResolvePath(remoteName).Download(localName, copyFlags, ri);
        }

        public override FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            return ResolvePath(remoteName).Upload(localName, copyFlags);
        }

        public override bool FileRemove(string remoteName)
        {
            return ResolvePath(remoteName).DeleteFile();
        }


        public override bool DirectoryCreate(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            path = path.TrimEnd('\\');
            return ResolvePath(Path.GetDirectoryName(path))
                .CreateFolder(Path.GetFileName(path));
        }

        public override bool DirectoryRemove(string remoteName)
        {
            return ResolvePath(remoteName).DeleteFolder();
        }


        public override CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlag extractIconFlag, out Icon icon)
        {
            icon = ResolvePath(remoteName).GetIcon();
            return icon != null ? CustomIconResult.Extracted : CustomIconResult.UseDefault;
        }

        public override void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {
            log.Trace("Command '{0}' for '{1}' {2}", operation, remoteName, origin.ToString().ToLower());
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
    }
}
