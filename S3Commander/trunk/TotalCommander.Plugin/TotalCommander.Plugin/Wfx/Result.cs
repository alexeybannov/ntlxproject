using System;
using System.Collections.Generic;
using System.Text;

namespace TotalCommander.Plugin.Wfx
{
    static class Result
    {
        /// <summary>
        /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
        /// </summary>
        /// <seealso cref="ITotalCommanderWfxPlugin.GetCustomIcon"/>
        public enum CustomIcon
        {
            /// <summary>
            /// No icon is returned. The calling app should show the default icon for this file type.
            /// </summary>
            UseDefault,

            /// <summary>
            /// An icon was returned. The icon must NOT be freed by the calling app.
            /// </summary>
            Extracted,

            /// <summary>
            /// An icon was returned. The icon MUST be destroyed by the calling app.
            /// </summary>
            ExtractedDestroy,

            /// <summary>
            /// This return value is only valid if <see cref="CustomIconFlag.Background"/> was NOT set. 
            /// It tells the calling app to show a default icon, and request the true icon in a background thread.
            /// </summary>
            Delayed
        }

        /// <summary>
        /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.FileExecute"/> method.
        /// </summary>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileExecute"/>
        public enum Execute
        {
            /// <summary>
            /// A (symbolic) link or .lnk file pointing to a different directory.
            /// </summary>
            SymLink = -2,

            /// <summary>
            /// Total Commander should download the file and execute it locally.
            /// </summary>
            YourSelf,

            /// <summary>
            /// The command was executed successfully.
            /// </summary>
            OK,

            /// <summary>
            /// Execution failed.
            /// </summary>
            Error,

            /// <summary>
            /// Default value, equal <see cref="ExecuteResult.Error"/>
            /// </summary>
            Default = Error
        }

        /// <summary>
        /// Defines constants that are returned by the 
        /// <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/>, 
        /// <see cref="ITotalCommanderWfxPlugin.FileGet"/> and
        /// <see cref="ITotalCommanderWfxPlugin.FilePut"/> methods.
        /// </summary>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileRenameMove"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FilePut"/>
        public enum FileOperation
        {
            /// <summary>
            /// The file was copied OK
            /// </summary>
            OK,

            /// <summary>
            /// The local file already exists, and resume isn't supported.
            /// </summary>
            Exists,

            /// <summary>
            /// The remote file couldn't be found or opened.
            /// </summary>
            NotFound,

            /// <summary>
            /// There was an error reading from the remote file.
            /// </summary>
            ReadError,

            /// <summary>
            /// There was an error writing to the local file, e.g. disk full.
            /// </summary>
            WriteError,

            /// <summary>
            /// Copying was aborted by the user (through ProgressProc).
            /// </summary>
            UserAbort,

            /// <summary>
            /// The operation is not supported (e.g. resume).
            /// </summary>
            NotSupported,

            /// <summary>
            /// The local file already exists, and resume is supported.
            /// </summary>
            ExistsResumeAllowed,

            /// <summary>
            /// Default value equal <see cref="FileOperationResult.NotSupported"/>
            /// </summary>
            Default = NotSupported
        }

        /// <summary>
        /// Defines constants that are returned by the <see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/>
        /// </summary>
        /// <seealso cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/>
        public enum PreviewBitmap
        {
            /// <summary>
            /// There is no preview bitmap.
            /// </summary>
            None = 0,

            /// <summary>
            /// The image was extracted and is returned.
            /// </summary>
            Extracted,

            /// <summary>
            /// Tells the caller to extract the image by itself.
            /// </summary>
            ExtractYourSelf,

            /// <summary>
            /// Tells the caller to extract the image by itself, and then delete the temporary image file.
            /// </summary>
            ExtractYourSelfAndDelete,

            /// <summary>
            /// This value must be ADDED to one of the above values if the caller should cache the image.
            /// </summary>
            Cache = 256
        }
    }
}
