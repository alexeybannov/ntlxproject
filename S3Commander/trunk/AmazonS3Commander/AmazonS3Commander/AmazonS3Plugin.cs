using System;
using System.Collections;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace AmazonS3Commander
{
    [TotalCommanderPlugin]
    public class AmazonS3Plugin : TotalCommanderWfxPlugin
    {
        private IFileSystem fileSystem;


        public override string PluginName
        {
            get { return "Amazon S3 Commander"; }
        }

        public override FindData FindFirst(string path, out IEnumerator enumerator)
        {
            enumerator = null;

            var file = fileSystem.ResolvePath(path);
            if (file != null)
            {
                enumerator = file.GetChildren().GetEnumerator();
                return FindNext(enumerator);
            }
            return FindData.NotOpen;
        }

        public override FindData FindNext(IEnumerator enumerator)
        {
            return enumerator != null && enumerator.MoveNext() ? 
                ToFindData((IFile)enumerator.Current) : 
                FindData.NoMoreFiles;
        }

        public override void FindClose(IEnumerator enumerator)
        {
            var disposable = enumerator as IDisposable;
            if (disposable != null) disposable.Dispose();
        }


        public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
        {
            var file = fileSystem.ResolvePath(remoteName);
            if (file != null) return file.Open(window);
            return base.ExecuteOpen(window, ref remoteName);
        }
        

        private FindData ToFindData(IFile file)
        {
            return new FindData(file.Name)
            {
                Attributes = file.Attributes,
                FileSize = file.FileSize,
                LastWriteTime = file.LastWriteTime
            };
        }
    }
}
