using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    public interface ITotalCommanderWfxPlugin
    {
        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.Init"/> is called when loading the plugin.
        /// The passed values should be stored in the plugin for later use.
        /// </summary>
        /// <param name="progress"><see cref="Progress"/> class, which contains progress functions.</param>
        /// <param name="logger"><see cref="Logger"/> class, which contains logging functions.</param>
        /// <param name="request"><see cref="Request"/> class, which contains request text functions.</param>
        /// <remarks>
        /// <see cref="ITotalCommanderWfxPlugin.Init"/> is NOT called when the user initially installs the plugin.
        /// The plugin DLL is loaded when the user enters the plugin root in Network Neighborhood.
        /// </remarks>
        void Init(Progress progress, Logger logger, Request request);

        
        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> is called to retrieve the first file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="path">
        /// Full path to the directory for which the directory listing has to be retrieved.
        /// Important: no wildcards are passed to the plugin!
        /// All separators will be backslashes, so you will need to convert them to forward slashes if your file system uses them!
        /// As root, a single backslash is passed to the plugin.
        /// All subdirs are built from the directory names the plugin returns through 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> and <see cref="ITotalCommanderWfxPlugin.FindNext"/>,
        /// separated by single backslashes, e.g. \Some server\c:\subdir
        /// </param>
        /// <param name="enumerator">
        /// The find enumerator. This enumerator will be passed to <see cref="ITotalCommanderWfxPlugin.FindNext"/> by the calling program.
        /// </param>
        /// <returns>
        /// <list type="number">
        /// <item><term>Return <see cref="FindData"/> class, which contains the file or directory details.
        /// Use the <see cref="FindData.Attributes"/> property set to <see cref="System.IO.FileAttributes.Directory"/> to distinguish files from directories.
        /// On Unix systems, you can | (or) the <see cref="FindData.Attributes"/> property with 0x80000000 and
        /// set the <see cref="FindData.Reserved0"/> property to the Unix file mode (permissions).</term></item>
        /// <item><term>Return <see cref="FindData.NoMoreFiles"/> if the directory exists, but it's empty (Totalcmd can open it, e.g. to copy files to it).</term></item>
        /// <item><term>Return <see cref="FindData.NotOpen"/> if the directory does not exist, and Total Commander will not try to open it.</term></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/> may be called directly with a subdirectory of the plugin!
        /// You cannot rely on it being called with the root \ after it is loaded.
        /// Reason: Users may have saved a subdirectory to the plugin in the Ctrl+D directory hotlist in a previous session with the plugin.
        /// </remarks>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/>
        FindData FindFirst(string path, out IEnumerator enumerator);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindNext"/> is called to retrieve the next file in a directory of the plugin's file system.
        /// </summary>
        /// <param name="enumerator">The find enumerator returned by <see cref="ITotalCommanderWfxPlugin.FindFirst"/>.</param>
        /// <returns>
        /// <list type="number">
        /// <item><term>Return <see cref="FindData"/> class, which contains the file or directory details.
        /// Use the <see cref="FindData.Attributes"/> property set to <see cref="System.IO.FileAttributes.Directory"/> to distinguish files from directories.
        /// On Unix systems, you can | (or) the <see cref="FindData.Attributes"/> property with 0x80000000 and
        /// set the <see cref="FindData.Reserved0"/> property to the Unix file mode (permissions).</term></item>
        /// <item><term>Return <see cref="FindData.NoMoreFiles"/> if there are no more files.</term></item>
        /// </list>
        /// </returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindClose"/>
        FindData FindNext(IEnumerator enumerator);
        
        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.FindClose"/> is called to end a 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> loop, 
        /// either after retrieving all files, or when the user aborts it.
        /// </summary>
        /// <param name="enumerator">The find enumerator returned by <see cref="ITotalCommanderWfxPlugin.FindFirst"/>.</param>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindFirst"/>
        /// <seealso cref="ITotalCommanderWfxPlugin.FindNext"/>
        void FindClose(IEnumerator enumerator);


        ExecuteResult FileExecute(TotalCommanderWindow window, string remoteName, string verb);

        FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri);

        FileOperationResult FileGet(string remoteName, string localName, CopyFlags copyFlags, RemoteInfo ri);

        FileOperationResult FilePut(string localName, string remoteName, CopyFlags copyFlags);

        bool FileRemove(string remoteName);

        bool DirectoryCreate(string path);

        bool DirectoryRemove(string remoteName);


        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetDefaultParams"/> is called immediately after <see cref="ITotalCommanderWfxPlugin.Init"/>.
        /// This function is new in version 1.3. It requires Total Commander >=5.51, but is ignored by older versions.
        /// </summary>
        /// <param name="defaultParam">This class of type <see cref="DefaultParam"/> currently contains the version number of the plugin interface,
        /// and the suggested location for the settings file (ini file).
        /// It is recommended to store any plugin-specific information either directly in that file, or in that directory under a different name.
        /// Make sure to use a unique header when storing data in this file, because it is shared by other file system plugins!
        /// If your plugin needs more than 1kbyte of data, you should use your own ini file because ini files are limited to 64k.
        /// </param>
        /// <remarks>
        /// This function is only called in Total Commander 5.51 and later. The plugin version will be >= 1.3.
        /// </remarks>
        void SetDefaultParams(DefaultParam defaultParam);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetFileAttributes"/> is called to set the (Windows-Style) file attributes of a file/dir.
        /// <see cref="ITotalCommanderWfxPlugin.FileExecute"/> is called for Unix-style attributes.
        /// </summary>
        /// <param name="remoteName">Name of the file/directory whose attributes have to be set.</param>
        /// <param name="attributes">New file attributes. These are a commbination of the following standard file attributes: 
        /// <see cref="FileAttributes.ReadOnly"/>, <see cref="FileAttributes.Hidden"/>, <see cref="FileAttributes.System"/>, <see cref="FileAttributes.Archive"/>.</param>
        /// <returns>Return true if successful, false if the function failed.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.FileExecute"/>
        bool SetFileAttributes(string remoteName, FileAttributes attributes);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetFileTime"/> is called to set the (Windows-Style) file times of a file/dir.
        /// </summary>
        /// <param name="remoteName">Name of the file/directory whose attributes have to be set.</param>
        /// <param name="creationTime">Creation time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastAccessTime">Last access time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastWriteTime">Last write time of the file. May be NULL to leave it unchanged. If your file system only supports one time, use this parameter!</param>
        /// <returns>Return true if successful, false if the function failed.</returns>
        bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);

        
        CustomIconResult GetCustomIcon(string remoteName, CustomIconFlag extractIconFlag, out Icon icon);

        PreviewBitmapResult GetPreviewBitmap(string remoteName, Size size, out Bitmap bitmap);

        
        void StatusInfo(string remoteName, StatusInfo info, StatusOperation operation);

        bool Disconnect(string disconnectRoot);
    }
}
