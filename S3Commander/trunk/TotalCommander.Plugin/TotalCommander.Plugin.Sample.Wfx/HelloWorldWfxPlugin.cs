using System;
using System.Collections;
using System.IO;
using TotalCommander.Plugin.Wfx;
using NI.Vfs;

namespace TotalCommander.Plugin.Sample.Wfx
{
    [TotalCommanderPlugin("Hello World Plugin")]
    public class HelloWorldWfxPlugin : TotalCommanderWfxPlugin
    {
        private IFileSystem fileSystem = new LocalFileSystem("D:\\");

        public override FindData FindFirst(string path, out IEnumerator enumerator)
        {
            var file = GetFile(path);
            if (file != null)
            {
                enumerator = file.GetChildren().GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return ToFindData((IFileObject)enumerator.Current);
                }
            }
            enumerator = null;
            return FindData.NoMoreFiles;
        }

        public override FindData FindNext(IEnumerator enumerator)
        {
            if (enumerator != null && enumerator.MoveNext())
            {
                return ToFindData((IFileObject)enumerator.Current);
            }
            return FindData.NoMoreFiles;
        }

        public override bool DirectoryCreate(string path)
        {
            try
            {
                GetFile(path).CreateFolder();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool DirectoryRemove(string remoteName)
        {
            try
            {
                GetFile(remoteName).Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override ExecuteResult ExecuteOpen(TotalCommanderWindow window, ref string remoteName)
        {
            var username = "Nikolay";
            var r = Request.MessageBox("Hello, World!");
            return ExecuteResult.OK;
        }

        private IFileObject GetFile(string path)
        {
            return path == "\\" ? fileSystem.Root : fileSystem.ResolveFile("D:" + path);
        }

        private FindData ToFindData(IFileObject file)
        {
            var findData = new FindData(Path.GetFileName(file.Name));
            var fileInfo = file.GetContent();
            if (file.Type == FileType.Folder) findData.Attributes |= FileAttributes.Directory;
            else findData.FileSize = fileInfo.Size;
            findData.LastWriteTime = fileInfo.LastModifiedTime;
            fileInfo.Close();
            return findData;
        }
    }
}
