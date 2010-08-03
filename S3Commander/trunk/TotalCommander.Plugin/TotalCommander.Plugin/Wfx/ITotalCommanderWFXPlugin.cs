using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// Represents file system plugins for Total Commander.
    /// File system plugins will show up in Network Neighborhood, not in the new file system.
    /// </summary>
    /// <remarks>
    /// <br />
    /// <strong>The minimum functions needed for a read-only (browsing) plugin are:</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.Init"/></term>
    /// <description>Initialize the plugin.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindFirst"/></term>
    /// <description>Retrieve the first file in a directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindNext"/></term>
    /// <description>Get the next file in the directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FindClose"/></term>
    /// <description>Close the search handle.</description>
    /// </item>
    /// </list>
    /// <br />
    /// <strong>The following optional functions allow to manipulate individual files.</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileGet"/></term>
    /// <description>Download a file from the plugin file system to a local disk.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FilePut"/></term>
    /// <description>Upload a file from the local disk to the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileRenameMove"/></term>
    /// <description>Copy, rename or move a file within the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileRemove"/></term>
    /// <description>Delete a file on the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.FileExecute"/></term>
    /// <description>Execute a command or launch a file on the plugin file system, or show properties.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.DirectoryCreate"/></term>
    /// <description>Create a new directory in the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.DirectoryRemove"/></term>
    /// <description>Remove a directory on the plugin file system.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.SetFileAttributes"/></term>
    /// <description>Set the file attributes of a file or directory.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.SetFileTime"/></term>
    /// <description>Set the file times.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.GetCustomIcon"/></term>
    /// <description>Extract icon for file list.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.GetPreviewBitmap"/></term>
    /// <description>Return a bitmap for thumbnail view.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.Disconnect"/></term>
    /// <description>For file systems requiring a connection: User pressed disconnect button.</description>
    /// </item>
    /// <item>
    /// <term><see cref="ITotalCommanderWfxPlugin.StatusInfo"/></term>
    /// <description>Informs the plugin that an operation is just going to start or end (purely informational).</description>
    /// </item>
    /// <item>
    /// <term>Content*</term>
    /// <description>These functions are almost identical to the Content* functions of the content plugin interface.</description>
    /// </item>
    /// </list>
    /// <br />
    /// <strong>There are also 3 classes which the plugin can use:</strong>
    /// <list type="table">
    /// <item>
    /// <term><see cref="Progress"/></term>
    /// <description>Use this to indicate the progress in percent of a single copy operation.</description>
    /// </item>
    /// <item>
    /// <term><see cref="Logger"/></term>
    /// <description>Use to add information to the log file, and to make the FTP toolbar appear.</description>
    /// </item>
    /// <item>
    /// <term><see cref="Request"/></term>
    /// <description>Request input from the user, e.g. a user name, password etc.</description>
    /// </item>
    /// </list>
    /// <br />
    /// <strong>How it works:</strong>
    /// <br /><br />
    /// When a user installs the plugin in Total Commander, the plugin is loaded and FsGetDefRootName is called without a previous call to FsInit.
    /// The name returned will be saved to wincmd.ini. Then the plugin will be unloaded.
    /// When the user enters Network Neighborhood, Totalcmd will enumerate all plugins listed in wincmd.ini without loading the plugins!
    /// A plugin will only be loaded when the user tries to enter the plugin root directory.
    /// It's also possible that a user jumps directly to a plugin subdirectory by using the 'directory hotlist' or 'directory history' functions in Totalcmd.
    /// <br /><br />
    /// When the plugin is loaded, Totalcmd tries to get the addresses for the above functions.
    /// If any of the minimum functions isn't implemented, loading of the plugin will fail.
    /// If any of the optional functions is missing, loading will succeed, but the functions (e.g. deletion) will not be available to the user.
    /// After retrieving the function addresses, Totalcmd will call FsInit to let the plugin know its number and the callback function addresses.
    /// <br /><br />
    /// The framework (Total Commander) will refresh the file list whenever the user enters any directory in the plugin's file system.
    /// The same procedure will also be executed if the framework wants to work with subdirectories, 
    /// e.g. while copying/moving/deleting files in a subdir selected by the user. 
    /// This is done by recursively calling FsFindFirst()...FsFindNext()...FsFindClose() for every directory encountered in the tree. 
    /// This system will be called FNC (findfirst-next-close) in this text. 
    /// <br /><br />
    /// For the plugin root, Totalcmd calls FsFindFirst() with the parameter Path set to "\". 
    /// The plugin should return all the items in the root, e.g. the drive letters of a remote machine, the available file systems etc. 
    /// When the returned item has the directory flag set, Totalcmd will use the name to build a subdirectory path. 
    /// Subdirectories are built by concatenating returned directory names separated by Backslashes, e.g. \drive1\c:\some\subdir.
    /// <br /><br />
    /// While downloading or remote-copying whole directory trees, the framework executes a complete FNC loop of a 
    /// subdir and stores the files in an internal list. Then it checks the list for files and copies these files, and in a 
    /// second loop it checks the list for directories, and if it encounters them, it recursively copies the subdirs. 
    /// This allows to recursively copy a whole tree.
    /// <br /><br />
    /// For counting the files in subdirs and for deleting files, 
    /// multiple open file handles are needed. You should therefore initialise a temporary structure whenever FsFindFirst() is called, 
    /// return its handle (pointer) to the framework, and delete it in FsFindClose(), using that same handle that is now returned to you. 
    /// It's important to know that there may be multiple open find handles a the same time, although great care is taken to avoid this.
    /// <br /><br />
    /// Some framework function may call other functions when the need arises - for instance, FsRemoveDir() is called during 
    /// moving of files in order to delete the directories that are no longer needed. 
    /// <br /><br />
    /// Here are some cases when you CAN'T rely on the FNC to get called (because it has already been called before):<br />
    /// - when copying some files in the currently active directory, and there are no directories selected for copying<br />
    /// - when viewing a file with F3
    /// <br /><br />
    /// If FsStatusInfo is implemented, the plugin will be informed every time an operation starts and ends. 
    /// No plugin functions except for FsInit and FsDisconnect will be called without an enclosing pair of FsStatusInfo calls.
    /// <br /><br />
    /// It is strongly recommended to start with an existing plugin source and modify it, e.g. with the very simple fsplugin sample source. 
    /// Then first implement FsInit, FsFindFirst, FsFindNext and FsFindClose to browse your file system. 
    /// When this works, you can add the other functions to add functionality like uploading and downloading.
    /// </remarks>
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
        /// <seealso cref="ITotalCommanderWfxPlugin.SetDefaultParams"/>
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

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.DirectoryCreate"/> is called to create a directory on the plugin's file system.
        /// </summary>
        /// <param name="path">
        /// Name of the directory to be created, with full path. 
        /// The name always starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> separated by backslashes.
        /// </param>
        /// <returns>Return true if the directory could be created, false if not.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.DirectoryRemove"/>
        bool DirectoryCreate(string path);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.DirectoryRemove"/> is called to remove a directory from the plugin's file system.
        /// </summary>
        /// <param name="remoteName">
        /// Name of the directory to be removed, with full path.
        /// The name always starts with a backslash, then the names returned by 
        /// <see cref="ITotalCommanderWfxPlugin.FindFirst"/>/<see cref="ITotalCommanderWfxPlugin.FindNext"/> separated by backslashes.
        /// </param>
        /// <returns>Return true if the directory could be removed, false if not.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.DirectoryCreate"/>
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
        /// <seealso cref="ITotalCommanderWfxPlugin.Init"/>
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
        /// <seealso cref="ITotalCommanderWfxPlugin.SetFileTime"/>
        bool SetFileAttributes(string remoteName, FileAttributes attributes);

        /// <summary>
        /// <see cref="ITotalCommanderWfxPlugin.SetFileTime"/> is called to set the (Windows-Style) file times of a file/dir.
        /// </summary>
        /// <param name="remoteName">Name of the file/directory whose attributes have to be set.</param>
        /// <param name="creationTime">Creation time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastAccessTime">Last access time of the file. May be NULL to leave it unchanged.</param>
        /// <param name="lastWriteTime">Last write time of the file. May be NULL to leave it unchanged. If your file system only supports one time, use this parameter!</param>
        /// <returns>Return true if successful, false if the function failed.</returns>
        /// <seealso cref="ITotalCommanderWfxPlugin.SetFileAttributes"/>
        bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);

        
        CustomIconResult GetCustomIcon(string remoteName, CustomIconFlag extractIconFlag, out Icon icon);

        PreviewBitmapResult GetPreviewBitmap(string remoteName, Size size, out Bitmap bitmap);

        
        void StatusInfo(string remoteName, StatusInfo info, StatusOperation operation);

        bool Disconnect(string disconnectRoot);
    }
}
