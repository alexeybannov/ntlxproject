using System.Drawing;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFile
    {
        FindData GetFileInfo();


        ExecuteResult Open(TotalCommanderWindow window, ref string link);

        ExecuteResult Properties(TotalCommanderWindow window, ref string link);


        bool CreateFolder(string name);

        bool Remove();


        CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon);

        PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap);
    }
}
