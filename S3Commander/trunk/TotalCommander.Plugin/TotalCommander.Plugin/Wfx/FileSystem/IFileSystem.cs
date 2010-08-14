
namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFileSystem
    {
        IFile ResolvePath(string path);

        bool Disconnect(string root);
    }
}
