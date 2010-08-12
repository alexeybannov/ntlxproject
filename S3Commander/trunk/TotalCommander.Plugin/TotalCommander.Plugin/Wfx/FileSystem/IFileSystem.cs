using System.Collections.Generic;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFileSystem
    {
        void Initialize(FileSystemContext context);

        IFile ResolvePath(string path);

        IEnumerator<IFile> GetFiles(string path);

        void StatusInfo(string path, StatusOrigin origin, StatusOperation operation);

        bool Disconnect(string root);
    }
}
