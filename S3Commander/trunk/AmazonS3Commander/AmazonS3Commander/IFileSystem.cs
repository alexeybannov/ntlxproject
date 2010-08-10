
namespace AmazonS3Commander
{
    interface IFileSystem
    {
        void Initialize(FileSystemContext context);

        IFile ResolvePath(string path);
    }
}
