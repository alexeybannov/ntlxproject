using System.Drawing;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFile
    {
        FindData Info
        {
            get;
        }

        IEnumerator<IFile> GetFiles();


        ExecuteResult Open(TotalCommanderWindow window, ref string link);

        ExecuteResult Properties(TotalCommanderWindow window, ref string link);


        bool CreateFolder(string name);

        bool Remove();


        CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon);

        PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap);


        void OperationBegin(StatusOperation operation);

        void OperationEnd(StatusOperation operation);
    }
}
