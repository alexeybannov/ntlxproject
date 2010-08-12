using System.Drawing;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    class File : IFile
    {
        public readonly static IFile Empty = new File();


        public virtual FindData GetFileInfo()
        {
            return FindData.NotOpen;
        }

        public virtual ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual bool CreateFolder(string name)
        {
            return false;
        }

        public virtual bool Remove()
        {
            return false;
        }


        public virtual CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            return CustomIconResult.UseDefault;
        }

        public virtual PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap)
        {
            return PreviewBitmapResult.None;
        }
    }
}
