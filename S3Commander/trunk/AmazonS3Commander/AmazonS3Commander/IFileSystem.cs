
namespace AmazonS3Commander
{
    interface IFileSystem
    {
        IFile ResolvePath(string path);
    }
}
