using System.Drawing;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public class FileBase : IFile
    {
        public readonly static IFile Empty = new FileBase();


        public FindData Info
        {
            get;
            protected set;
        }

        public FileBase()
        {
            Info = FindData.NotOpen;
        }

        public virtual IEnumerator<FindData> GetFiles()
        {
            return null;
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


        public virtual void OperationBegin(StatusOperation operation)
        {

        }

        public virtual void OperationEnd(StatusOperation operation)
        {

        }
    }
}
