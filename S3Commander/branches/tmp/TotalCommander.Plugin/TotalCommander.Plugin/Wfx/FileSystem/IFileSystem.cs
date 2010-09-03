
namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFileSystem
    {
        IFile ResolvePath(string path);

        void OperationInfo(string remoteDir, StatusOrigin origin, StatusOperation operation);

        bool Disconnect(string root);
    }
}
