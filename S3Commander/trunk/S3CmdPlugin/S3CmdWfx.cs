using System;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;
using System.Windows.Forms;

namespace S3CmdPlugin
{
    [TotalCommanderPlugin("S3CmdX")]
    [ResourcePluginIcon(typeof(S3CmdWfx), "S3CmdPlugin.Resources.PluginResources", "amazon")]
    public class S3CmdWfx : FileSystemPlugin
    {
        private IPathResolver pathResolver = new PluginRoot();


        public override string Name
        {
            get { return PluginResources.ProductName; }
        }

        public S3CmdWfx()
            : base()
        {

        }

        public override object FindFirst(string Path, ref FindData FindData)
        {
            try
            {
                var browsable = pathResolver.ResolvePath(Path) as IDirectory;
                if (browsable != null)
                {
                    browsable.Reset();
                    if (browsable.MoveNext()) FindData = browsable.Current.FindData;
                }
                return browsable;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return null;
            }
        }

        public override bool FindNext(object Status, ref FindData FindData)
        {
            try
            {
                var browsable = Status as IDirectory;
                if (browsable != null && browsable.MoveNext())
                {
                    FindData = browsable.Current.FindData;
                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return false;
        }

        public override void FindClose(object Status)
        {
            try
            {
                var disposable = Status as IDisposable;
                if (disposable != null) disposable.Dispose();

                base.FindClose(Status);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        public override ExecExitCode ExecuteFile(IntPtr hMainWin, ref string RemoteName, string Verb)
        {
            try
            {
                var file = pathResolver.ResolvePath(RemoteName) as IFile;
                if (file == null) return base.ExecuteFile(hMainWin, ref RemoteName, Verb);

                var mainWindow = new MainWindow(hMainWin);
                var comparison = StringComparison.InvariantCultureIgnoreCase;

                if (Verb.StartsWith("open", comparison)) return file.Open(mainWindow);
                if (Verb.StartsWith("properties", comparison)) return file.Properties(mainWindow);
                if (Verb.StartsWith("chmod ", comparison)) return file.ChMod(mainWindow, Verb.Substring(6));
                if (Verb.StartsWith("quote ", comparison)) return file.Quote(mainWindow, Verb.Substring(6));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return ExecExitCode.Error;
        }

        public override bool MkDir(string Path)
        {
            try
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    Path = Path.TrimEnd('\\');
                    var position = Path.LastIndexOf('\\') + 1;
                    if (0 < position && position < Path.Length)
                    {
                        var dir = pathResolver.ResolvePath(Path.Substring(0, position)) as IDirectory;
                        if (dir != null) return dir.Create(Path.Substring(position));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return false;
        }

        public override bool RemoveDir(string RemoteName)
        {
            try
            {
                var dir = pathResolver.ResolvePath(RemoteName) as IDirectory;
                if (dir != null) return dir.Remove();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return false;
        }

        protected override ExecExitCode ExecuteCommand(IntPtr hMainWin, ref string RemoteName, string command)
        {
            return base.ExecuteCommand(hMainWin, ref RemoteName, command);
        }

        public override FileSystemExitCode RenMovFile(string OldName, string NewName, bool Move, bool OverWrite, RemoteInfo info)
        {
            return base.RenMovFile(OldName, NewName, Move, OverWrite, info);
        }

        public override bool DeleteFile(string RemoteName)
        {
            try
            {
                if (string.IsNullOrEmpty(RemoteName)) return false;
                if (RemoteName.EndsWith("\\")) return RemoveDir(RemoteName);

                var file = pathResolver.ResolvePath(RemoteName) as IFile;
                if (file != null) return file.Delete();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return false;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.ToString(), Name);
        }
    }
}
