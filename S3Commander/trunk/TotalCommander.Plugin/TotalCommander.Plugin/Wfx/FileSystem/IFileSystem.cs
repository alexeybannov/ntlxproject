
namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFileSystem
    {
        void Initialize(FileSystemContext context);

        IFile ResolvePath(string path);

        bool Disconnect(string root);
    }
}
