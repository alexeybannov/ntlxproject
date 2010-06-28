using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Forms;
using HgCo.WindowsLive.SkyDrive.TotalCommander.Plugins.Forms;
using Tools.TotalCommanderT;

// TODO: I find that an exception is coming from C++: Remoting loses connection to SkyDriveWfx instance after a while.
// TODO: Try to solve disconnect

namespace HgCo.WindowsLive.SkyDrive.TotalCommander.Plugins
{
    /// <summary>
    /// Represents a Total Commander File System Plugin (.wfx) for Windows Live SkyDrive.
    /// </summary>
    [TotalCommanderPlugin("SkyDriveExplorer")]
    [ResourcePluginIcon(typeof(SkyDriveWfx), "HgCo.WindowsLive.SkyDrive.TotalCommander.Plugins.Resources.SkyDriveWfx", "mini_storage")]
    public class SkyDriveWfx : FileSystemPlugin
    {
        #region Fields

        /// <summary>
        /// The timeout for SkyDrive WebClient.
        /// </summary>
        public const int SkyDriveWebClientTimeout = 15 * 1000;

        /// <summary>
        /// The path URL of the root webfolder.
        /// </summary>
        private static readonly string RootWebFolderPathUrl = WebFolderItemInfo.PathUrlSegmentDelimiter.ToString();

        /// <summary>
        /// The root webfolder. The dummy webfolder used to cache the whole webfolder tree.
        /// </summary>
        private WebFolderInfo WebFolderRoot { get; set; }

        /// <summary>
        /// The webfolder listed since the last FindFirst call.
        /// </summary>
        private WebFolderInfo WebFolderLastListed { get; set; }

        /// <summary>
        /// The webfolder which subitems were changed since the last FindFirst call.
        /// </summary>
        private WebFolderInfo WebFolderSubItemsLastChanged { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of SkyDriveWfx displayed in Total Commander.
        /// </summary>
        /// <value>The name of SkyDriveWfx.</value>
        public override string Name
        {
            get { return "SkyDrive Explorer"; }
        }

        /// <summary>
        /// Gets or sets the session for SkyDriveWebClient.
        /// </summary>
        /// <value>The session for SkyDriveWebClient.</value>
        private WebSession Session { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SkyDriveWfx"/> class.
        /// </summary>
        public SkyDriveWfx()
        {
            WebFolderRoot = new WebFolderInfo
            {
                Name = "ROOT",
                ContentType = WebFolderContentType.Documents,
                ShareType = WebFolderItemShareType.Private,
                PathUrl = RootWebFolderPathUrl
            };
            WebFolderLastListed = WebFolderRoot;
        }

        #endregion

        #region Public Methods

        #region Pre- and PostOperation Related Methods

        /// <summary>
        /// OnOperationStarting is called just as an information to the plugin that a certain operation starts.
        /// </summary>
        /// <param name="e">The <see cref="Tools.TotalCommanderT.OperationEventArgs"/> instance containing the event data.</param>
        public override void OnOperationStarting(OperationEventArgs e)
        {
            base.OnOperationStarting(e);
        }

        /// <summary>
        /// OnOperationStatusChanged is called just as an information to the plugin that a certain operation starts or ends.
        /// </summary>
        /// <param name="e">The <see cref="Tools.TotalCommanderT.OperationEventArgs"/> instance containing the event data.</param>
        public override void OnOperationStatusChanged(OperationEventArgs e)
        {
            base.OnOperationStatusChanged(e);
        }

        /// <summary>
        /// OnOperationStarting is called just as an information to the plugin that a certain operation ends.
        /// </summary>
        /// <param name="e">The <see cref="Tools.TotalCommanderT.OperationEventArgs"/> instance containing the event data.</param>
        public override void OnOperationFinished(OperationEventArgs e)
        {
            base.OnOperationFinished(e);
        }

        #endregion

        #region Operation Connection Related Methods

        ///// <summary>
        ///// Disconnects from SkyDrive.
        ///// </summary>
        ///// <param name="DisconnectRoot">The disconnect root.</param>
        ///// <returns><c>true</c> if disconnection was successful; otherwise <c>false</c>.</returns>
        //public override bool Disconnect(string DisconnectRoot)
        //{
        //    Session = null;
        //    WebFolderRoot.SubItems = null;
        //    return true;
        //}

        #endregion

        #region Operation List Related Methods
        
        /// <summary>
        /// FindFirst is called to retrieve the first webfolderitem in a webfolder of SkyDrive's file system.
        /// </summary>
        /// <param name="Path">The full path to the webfolder of which items are to be listed.</param>
        /// <param name="FindData">The FindData structure used to retrieve webfolderitem info.</param>
        /// <returns>An enumerator for the list of webfolderitems.</returns>
        public override object FindFirst(string Path, ref FindData FindData)
        {
            IEnumerator enumeratorWebFolderItem = null;
            
            if (WebFolderSubItemsLastChanged != null)
            {
                WebFolderSubItemsLastChanged.SubItems = null;
                WebFolderSubItemsLastChanged = null;
            }

            string folderNamePath = Path;
            WebFolderInfo webFolder = GetWebFolderByNamePath(folderNamePath, WebFolderRoot);

            if (webFolder != null)
            {
                if (webFolder.SubItems == null || webFolder == WebFolderLastListed)
                    webFolder.SubItems = GetWebFolderSubItems(webFolder);

                if (webFolder.SubItems != null && webFolder.SubItems.Count > 0)
                {
                    enumeratorWebFolderItem = webFolder.SubItems.GetEnumerator();
                    if (enumeratorWebFolderItem.MoveNext())
                        FindData = FillFindData(enumeratorWebFolderItem.Current as WebFolderItemInfo);
                }

                WebFolderLastListed = webFolder;
            }

            return enumeratorWebFolderItem;
        }

        /// <summary>
        /// FindNext is called to retrieve the next webfolderitem in a webfolder of SkyDrive's file system.
        /// </summary>
        /// <param name="Status">The enumerator returned by FindFirst method.</param>
        /// <param name="FindData">The FindData structure used to retreive webfolderitem info.</param>
        /// <returns><c>true</c> if there are more webfolderitems to return, otherwise <c>false</c>.</returns>
        public override bool FindNext(object Status, ref FindData FindData)
        {
            bool hasMore = false;
            IEnumerator enumeratorWebFolderItem = Status as IEnumerator;
            if (enumeratorWebFolderItem != null)
            {
                if (enumeratorWebFolderItem.MoveNext())
                {
                    FindData = FillFindData(enumeratorWebFolderItem.Current as WebFolderItemInfo);
                    hasMore = true;
                }
            }
            return hasMore;
        }

        /// <summary>
        /// FindClose is called to end a FindFirst/FindNext loop, 
        /// either after retrieving all files, or when the user aborts it.
        /// </summary>
        /// <param name="Status">The enumerator returned by FindFirst method.</param>
        public override void FindClose(object Status)
        {
            base.FindClose(Status);

            IDisposable disposable = Status as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        /// <summary>
        /// ExctractCustomIcon is called when a webfolderitem is displayed in the file list. 
        /// It can be used to specify a custom icon for that webfolderitem.
        /// </summary>
        /// <param name="RemoteName">The full name of the webfolderitem to be displayed.</param>
        /// <param name="ExtractFlags">Can be combination of the <see cref="IconExtractFlags"/> values.</param>
        /// <param name="TheIcon">The webfolderitem icon.</param>
        /// <returns>One of <see cref="IconExtractResult"/> values.</returns>
        public override IconExtractResult ExctractCustomIcon(ref string RemoteName, IconExtractFlags ExtractFlags, ref System.Drawing.Icon TheIcon)
        {
            string namePath = RemoteName;
            WebFolderItemInfo webFolderItem = GetWebFolderItemByNamePath(namePath, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFolderItem != null && wcSkyDrive != null)
                try
                {
                    System.Drawing.Bitmap bmpIcon = 
                        wcSkyDrive.DownloadWebFolderItemIcon(webFolderItem.WebIcon) as System.Drawing.Bitmap;
                    if (bmpIcon != null)
                    {
                        IntPtr handleIcon = bmpIcon.GetHicon();
                        TheIcon = System.Drawing.Icon.FromHandle(handleIcon);
                        return IconExtractResult.Extracted;
                    }
                }
                catch { }

            return IconExtractResult.UseDefault;
        }

        /// <summary>
        /// GetPreviewBitmap is called when a webfolderitem is displayed in thumbnail view. 
        /// It can be used to return a custom bitmap for that webfolderitem.
        /// </summary>
        /// <param name="RemoteName">The full name of the webfolderitem to be displayed.</param>
        /// <param name="width">The maximum width of the preview bitmap.</param>
        /// <param name="height">The maximum height of the preview bitmap.</param>
        /// <returns>The preview bitmap.</returns>
        public override BitmapResult GetPreviewBitmap(string RemoteName, int width, int height)
        {
            BitmapResult bmpResult = null;

            string namePath = RemoteName;
            WebFolderItemInfo webFolderItem = GetWebFolderItemByNamePath(namePath, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFolderItem != null && wcSkyDrive != null)
                try
                {
                    WebFolderItemInfo webFolderItemClone = (WebFolderItemInfo)webFolderItem.Clone();

                    wcSkyDrive.GetWebFolderItem(webFolderItemClone);
                    System.Drawing.Image imgWebFolderItemClone = wcSkyDrive.DownloadWebFolderItemIcon(webFolderItemClone.WebIcon);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.Clear(System.Drawing.Color.White);
                        g.DrawImage(
                            imgWebFolderItemClone,
                            0, 0,
                            Math.Min(width, imgWebFolderItemClone.Width),
                            Math.Min(height, imgWebFolderItemClone.Height));
                    }
                    bmpResult = new BitmapResult(bmp);
                    bmpResult.Cache = true;
                }
                catch { }
            
            return bmpResult;
        }

        #endregion

        #region Operation Directory Related Methods

        /// <summary>
        /// MkDir is called to create a webfolder on SkyDrive's file system.
        /// </summary>
        /// <param name="Path">The full name of the webfolder to be created.</param>
        /// <returns><c>true</c>, if the webfolder could be created; otherwise, <c>false</c>.</returns>
        public override bool MkDir(string Path)
        {
            bool success = false;
            
            string folderName = System.IO.Path.GetFileName(Path);
            string folderNamePath = Path;
            string folderNamePathParent = System.IO.Path.GetDirectoryName(Path);
            WebFolderItemInfo webFolderItem = GetWebFolderItemByNamePath(folderNamePath, WebFolderRoot);
            WebFolderInfo webFolderParent = GetWebFolderByNamePath(folderNamePathParent, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFolderItem == null && webFolderParent != null && wcSkyDrive != null)
                try
                {
                    if (IsRootPath(webFolderParent.PathUrl))
                    {
                        SkyDriveWfxNewRootFolderForm formNewRootFolder = new SkyDriveWfxNewRootFolderForm();
                        formNewRootFolder.ShowDialog();

                        wcSkyDrive.CreateRootWebFolder(folderName, formNewRootFolder.SelectedShareType);

                        if (formNewRootFolder.SelectedContentType != WebFolderContentType.Documents)
                        {
                            webFolderParent.SubItems = null;
                            WebFolderInfo webFolderNew = GetWebFolderByNamePath(folderNamePath, WebFolderRoot);
                            wcSkyDrive.ChangeRootWebFolderContentType(webFolderNew, formNewRootFolder.SelectedContentType);
                        }
                    }
                    else
                    {
                        wcSkyDrive.CreateSubWebFolder(folderName, webFolderParent);
                    }

                    MarkWebFolderSubItemsChanged(webFolderParent);
                    success = true;
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();
                }

            return success;
        }

        /// <summary>
        /// RemoveDir is called to remove a webfolder from SkyDrive's file system.
        /// </summary>
        /// <param name="RemoteName">The full name of the webfolder to be removed.</param>
        /// <returns><c>true</c> if the webfolder could be removed; otherwise, <c>false</c>.</returns>
        public override bool RemoveDir(string RemoteName)
        {
            bool success = false;
            
            string folderNamePath = RemoteName;
            string folderNamePathParent = System.IO.Path.GetDirectoryName(RemoteName);
            WebFolderInfo webFolder = GetWebFolderByNamePath(folderNamePath, WebFolderRoot);
            WebFolderInfo webFolderParent = GetWebFolderByNamePath(folderNamePathParent, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFolder != null && webFolderParent != null && wcSkyDrive != null)
                try
                {
                    wcSkyDrive.DeleteWebFolder(webFolder);

                    MarkWebFolderSubItemsChanged(webFolderParent);
                    success = true;
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();
                }

            return success;
        }

        #endregion

        #region Operation File Related Methods

        /// <summary>
        /// GetFile is called to transfer a webfile from SkyDrive's file system to the normal/local file system.
        /// </summary>
        /// <param name="RemoteName">The full name of the webfile to be transfered.</param>
        /// <param name="LocalName">The full name of the local file where webfile needs to be transfered.</param>
        /// <param name="CopyFlags">Can be combination of the <see cref="CopyFlags"/> values.</param>
        /// <param name="info">It contains information about the webfile which was previously retrieved via <see cref="FindFirst"/>/<see cref="FindNext"/>.</param>
        /// <returns>One of the <see cref="FileSystemExitCode"/> values.</returns>
        public override FileSystemExitCode GetFile(string RemoteName, ref string LocalName, CopyFlags CopyFlags, RemoteInfo info)
        {
            string fileNamePath = RemoteName;
            string folderNamePathParent = System.IO.Path.GetDirectoryName(RemoteName);
            System.IO.FileInfo fiLocal = new System.IO.FileInfo(LocalName);

            if ((CopyFlags & CopyFlags.Overwrite) != CopyFlags.Overwrite && fiLocal.Exists)
                return FileSystemExitCode.FileExists;

            WebFileInfo webFile = GetWebFileByNamePath(fileNamePath, WebFolderRoot);
            WebFolderInfo webFolderParent = GetWebFolderByNamePath(folderNamePathParent, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFile != null && webFolderParent != null && wcSkyDrive != null)
                try
                {
                    using (System.IO.Stream sr = wcSkyDrive.DownloadWebFile(webFile))
                    using (System.IO.FileStream fsw = new System.IO.FileStream(LocalName, System.IO.FileMode.OpenOrCreate))
                    {
                        byte[] buffer = new byte[64 * 1024];
                        int count = 0;
                        while ((count = sr.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            try
                            {
                                fsw.Write(buffer, 0, count);
                            }
                            catch
                            {
                                return FileSystemExitCode.WriteError;
                            }
                            int percentDone = (int)((info.Size > 0 ? (fsw.Length / (decimal)info.Size) : 0M) * 100);
                            if (ProgressProc(RemoteName, LocalName, percentDone))
                                return FileSystemExitCode.UserAbort;
                        }
                    }
                    return FileSystemExitCode.OK;
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();

                    return FileSystemExitCode.ReadError;
                }
            return FileSystemExitCode.FileNotFound;
        }

        /// <summary>
        /// PutFile is called to transfer a file from the normal/local file system  to SkyDrive's file system.
        /// </summary>
        /// <param name="LocalName">The full name of the local file to be transfered.</param>
        /// <param name="RemoteName">The full name of the webfile where local file needs to be transfered.</param>
        /// <param name="CopyFlags">Can be combination of the <see cref="CopyFlags"/> values.</param>
        /// <returns>One of the <see cref="FileSystemExitCode"/> values.</returns>
        public override FileSystemExitCode PutFile(string LocalName, ref string RemoteName, CopyFlags CopyFlags)
        {
            System.IO.FileInfo fiLocal = new System.IO.FileInfo(LocalName); 
            string fileNamePath = RemoteName;
            string folderNamePathParent = System.IO.Path.GetDirectoryName(RemoteName);
            WebFolderInfo webFolderParent = GetWebFolderByNamePath(folderNamePathParent, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();
            bool isUserAborted = false;

            if (fiLocal.Exists && webFolderParent != null && wcSkyDrive != null)
                try
                {
                    if ((CopyFlags & CopyFlags.Overwrite) != CopyFlags.Overwrite)
                    {
                        WebFileInfo webFile = GetWebFileByNamePath(fileNamePath, webFolderParent);
                        if (webFile != null)
                            return FileSystemExitCode.FileExists;
                    }

                    wcSkyDrive.UploadWebFileProgressChanged += new EventHandler<UploadWebFileProgressChangedEventArgs>(delegate(object sender, UploadWebFileProgressChangedEventArgs e)
                        {
                            if (ProgressProc(LocalName, fileNamePath, e.ProgressPercentage))
                            {
                                isUserAborted = true;
                                throw new Exception();
                            }
                        });
                    wcSkyDrive.Timeout = Int32.MaxValue;
                    wcSkyDrive.UploadWebFile(LocalName, webFolderParent);

                    MarkWebFolderSubItemsChanged(webFolderParent);
                    return FileSystemExitCode.OK;
                }
                catch (Exception ex)
                {
                    if (isUserAborted)
                        return FileSystemExitCode.UserAbort;
                    else
                    {
                        SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                        formError.ShowDialog();

                        return FileSystemExitCode.WriteError;
                    }
                }
            return FileSystemExitCode.FileNotFound;
        }

        /// <summary>
        /// RenMovFile is called to transfer (copy or move) or rename a webfolderitem within SkyDrive's file system.
        /// </summary>
        /// <param name="OldName">The full name of the source webfolderitem.</param>
        /// <param name="NewName">The full name of the destination webfolderitem.</param>
        /// <param name="Move"><c>true</c> if webfolderitem needs to be moved to the new location and name; otherwise <c>false</c>.</param>
        /// <param name="OverWrite"><c>true</c> if target webfolderitem should be overwritten; otherwise <c>false</c>.</param>
        /// <param name="info">It contains information about the webfolderitem which was previously retrieved via <see cref="FindFirst"/>/<see cref="FindNext"/>.</param>
        /// <returns>One of the <see cref="FileSystemExitCode"/> values.</returns>
        public override FileSystemExitCode RenMovFile(string OldName, string NewName, bool Move, bool OverWrite, RemoteInfo info)
        {
            string namePathOld = OldName;
            string namePathNew = NewName;
            string folderNamePathParentOld = System.IO.Path.GetDirectoryName(OldName);
            string folderNamePathParentNew = System.IO.Path.GetDirectoryName(NewName);
            WebFolderItemInfo webFolderItemOld = GetWebFolderItemByNamePath(namePathOld, WebFolderRoot);
            WebFolderItemInfo webFolderItemNew = GetWebFolderItemByNamePath(namePathNew, WebFolderRoot);
            WebFolderInfo webFolderParentOld = GetWebFolderByNamePath(folderNamePathParentOld, WebFolderRoot);
            WebFolderInfo webFolderParentNew = GetWebFolderByNamePath(folderNamePathParentNew, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFolderParentOld != null && webFolderParentNew != null && wcSkyDrive != null)
                try
                {
                    if (!OverWrite && webFolderItemNew != null)
                        return FileSystemExitCode.FileExists;

                    if (Move)
                    {
                        // If only the item should be renamed
                        if (webFolderParentOld == webFolderParentNew)
                        {
                            string nameNew = System.IO.Path.GetFileName(namePathNew); 
                            if (webFolderItemOld.ItemType == WebFolderItemType.File)
                                nameNew = System.IO.Path.GetFileNameWithoutExtension(nameNew);

                            wcSkyDrive.RenameWebFolderItem(webFolderItemOld, nameNew);

                            MarkWebFolderSubItemsChanged(webFolderParentOld);
                            return FileSystemExitCode.OK;
                        }
                        // Move to/from the root is not allowed!
                        else if (!IsRootPath(folderNamePathParentOld) && !IsRootPath(folderNamePathParentNew))
                        {
                            wcSkyDrive.MoveSubWebFolderItem(webFolderItemOld, webFolderParentNew);

                            MarkWebFolderSubItemsChanged(webFolderParentOld);
                            MarkWebFolderSubItemsChanged(webFolderParentNew);
                        }
                    }
                    // Copy to/from the root is not allowed!
                    else if (!IsRootPath(folderNamePathParentOld) && !IsRootPath(folderNamePathParentNew))
                    {
                        WebFileInfo webFileOld = webFolderItemOld as WebFileInfo;

                        wcSkyDrive.CopyWebFile(webFileOld, webFolderParentNew);

                        MarkWebFolderSubItemsChanged(webFolderParentOld);
                        MarkWebFolderSubItemsChanged(webFolderParentNew);
                        return FileSystemExitCode.OK;
                    }

                    return FileSystemExitCode.NotSupported;
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();

                    return FileSystemExitCode.WriteError;
                }

            return FileSystemExitCode.NotSupported;
        }

        /// <summary>
        /// DeleteFile is called to delete a webfile from SkyDrive's file system.
        /// </summary>
        /// <param name="RemoteName">The full name of the webfile to be deleted.</param>
        /// <returns><c>true</c> if the webfile could be deleted; otherwise, <c>false</c>.</returns>
        public override bool DeleteFile(string RemoteName)
        {
            bool success = false;

            string fileNamePath = RemoteName;
            string folderNamePathParent = System.IO.Path.GetDirectoryName(RemoteName);
            WebFileInfo webFile = GetWebFileByNamePath(fileNamePath, WebFolderRoot);
            WebFolderInfo webFolderParent = GetWebFolderByNamePath(folderNamePathParent, WebFolderRoot);
            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();

            if (webFile != null && webFolderParent != null && wcSkyDrive != null)
                try
                {
                    wcSkyDrive.DeleteWebFile(webFile);

                    MarkWebFolderSubItemsChanged(webFolderParent);
                    success = true;
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();
                }

            return success;
        }

        /// <summary>
        /// ExecuteFile is called to execute a command on a webfolderitem on the SkyDrive's file system. 
        /// It is also called to show a SkyDrive configuration dialog when the user right clicks on the plugin root and chooses 'properties'.
        /// </summary>
        /// <param name="hMainWin">The parent window's handler.</param>
        /// <param name="RemoteName">The full name of the webfolderitem to be executed/show its properties.</param>
        /// <param name="Verb">The command to execute on the webfolderitem: open, properties, chmod, or quote.</param>
        /// <returns>One of <see cref="ExecExitCode"/> values.</returns>
        public override ExecExitCode ExecuteFile(IntPtr hMainWin, ref string RemoteName, string Verb)
        {
            return base.ExecuteFile(hMainWin, ref RemoteName, Verb);
        }

        /// <summary>
        /// ShowFileInfo is called to show a webfolderitem properties.
        /// </summary>
        /// <param name="hMainWin">The handle to parent window.</param>
        /// <param name="RemoteName">The full name of the webfolderitem to be showed.</param>
        /// <returns>One of <see cref="ExecExitCode"/> values.</returns>
        protected override ExecExitCode ShowFileInfo(IntPtr hMainWin, string RemoteName)
        {
            string namePath = RemoteName;

            if (IsRootPath(namePath))
                LogOn();
            else
            {
                WebFolderItemInfo webFolderItem = GetWebFolderItemByNamePath(namePath, WebFolderRoot);
                SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();
                if (webFolderItem != null && wcSkyDrive != null)
                    try
                    {
                        WebFolderItemInfo webFolderItemClone = (WebFolderItemInfo)webFolderItem.Clone();
                        wcSkyDrive.GetWebFolderItem(webFolderItemClone);

                        System.Drawing.Image imgWebFolderItemClone = wcSkyDrive.DownloadWebFolderItemIcon(webFolderItemClone.WebIcon);
                        if (webFolderItemClone.ItemType == WebFolderItemType.Folder)
                        {
                            SkyDriveWfxFolderInfoForm formFolderInfo = new SkyDriveWfxFolderInfoForm((WebFolderInfo)webFolderItemClone, imgWebFolderItemClone);
                            formFolderInfo.Show();
                        }
                        else if (webFolderItemClone.ItemType == WebFolderItemType.File)
                        {
                            SkyDriveWfxFileInfoForm formFileInfo = new SkyDriveWfxFileInfoForm((WebFileInfo)webFolderItemClone, imgWebFolderItemClone);
                            formFileInfo.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                        formError.ShowDialog();

                        return ExecExitCode.Error;
                    }
            }
            return ExecExitCode.OK;
        }

        #endregion
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Logs on to SkyDrive.
        /// </summary>
        /// <returns>The session if log on was successful; otherwise, null.</returns>
        private void LogOn()
        {
            SkyDriveWfxLogOnForm formLogOn = new SkyDriveWfxLogOnForm();
            if (formLogOn.ShowDialog() == DialogResult.OK)
            {
                Session = formLogOn.Session;
                WebFolderRoot.SubItems = null;
                //LogProc(LogKind.Connect, String.Format(CultureInfo.InvariantCulture, @"CONNECT \SkyDriveExplorer", Name));
            }
        }

        /// <summary>
        /// Gets a SkyDrive WebClient (logs on if required).
        /// </summary>
        /// <returns>The SkyDrive WebClient.</returns>
        private SkyDriveWebClient GetSkyDriveWebClient()
        {
            SkyDriveWebClient wcSkyDrive = null;

            if (Session == null)
                LogOn();

            if (Session != null)
            {
                wcSkyDrive = new SkyDriveWebClient(Session);
                wcSkyDrive.Timeout = SkyDriveWebClientTimeout;
            }
            
            return wcSkyDrive;
        }

        /// <summary>
        /// Splits a name path into segments.
        /// </summary>
        /// <param name="namePath">The name path to split.</param>
        /// <returns>The list of name path segments.</returns>
        private string[] SplitNamePath(string namePath)
        {
            string[] pathNameSegments = null;
            if (!String.IsNullOrEmpty(namePath))
                pathNameSegments = namePath.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ?
                    namePath.Substring(1).Split(System.IO.Path.DirectorySeparatorChar) :
                    namePath.Split(System.IO.Path.DirectorySeparatorChar);
            return pathNameSegments;
        }

        /// <summary>
        /// Joins a list of segments into name path.
        /// </summary>
        /// <param name="namePathSegments">The list of name path segments.</param>
        /// <returns>The joined name path.</returns>
        private string JoinNamePathSegments(string[] namePathSegments)
        {
            return String.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}",
                System.IO.Path.DirectorySeparatorChar,
                String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), namePathSegments));
        }
        /// <summary>
        /// Joins a list of segments into name path.
        /// </summary>
        /// <param name="namePathSegments">The list of name path segments.</param>
        /// <param name="startIndex">The first element of name path segments to use in join.</param>
        /// <param name="count">The number of elements of name path segments to use in join.</param>
        /// <returns>The joined name path.</returns>
        private string JoinNamePathSegments(string[] namePathSegments, int startIndex, int count)
        {
            return String.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}",
                System.IO.Path.DirectorySeparatorChar,
                String.Join(
                    System.IO.Path.DirectorySeparatorChar.ToString(), 
                    namePathSegments, 
                    startIndex, 
                    count));
        }

        /// <summary>
        /// Joins a list of segments into path URL.
        /// </summary>
        /// <param name="pathUrlSegments">The list of path URL segments.</param>
        /// <param name="startIndex">The first element of path URL segments to use in join.</param>
        /// <param name="count">The number of elements of path URL segments to use in join.</param>
        /// <returns>The joined path URL.</returns>
        private string JoinPathUrlSegments(string[] pathUrlSegments, int startIndex, int count)
        {
            return String.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}",
                WebFolderItemInfo.PathUrlSegmentDelimiter,
                String.Join(
                    WebFolderItemInfo.PathUrlSegmentDelimiter.ToString(),
                    pathUrlSegments,
                    startIndex,
                    count));
        }

        /// <summary>
        /// Determines whether the specified path is the root.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if the specified path is the root; otherwise, <c>false</c>.</returns>
        private bool IsRootPath(string path)
        {
            return (String.IsNullOrEmpty(path) ||
                path.Equals(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.InvariantCultureIgnoreCase) ||
                path.Equals(WebFolderItemInfo.PathUrlSegmentDelimiter.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Fills a FindData structure with the given webfolderitem.
        /// </summary>
        /// <param name="webFolderItem">The webfolderitem.</param>
        /// <returns>The filled FindData structure.</returns>
        private FindData FillFindData(WebFolderItemInfo webFolderItem)
        {
            FindData fd = new FindData();
            if (webFolderItem != null)
            {
                switch (webFolderItem.ItemType)
                {
                    case WebFolderItemType.Folder:
                        fd.FileName = webFolderItem.Name;
                        fd.Attributes = FileAttributes.Directory;
                        break;
                    case WebFolderItemType.File:
                        fd.FileName = ((WebFileInfo)webFolderItem).FullName;
                        fd.Attributes = FileAttributes.Normal;
                        break;
                    default:
                        break;
                }
                fd.AccessTime = webFolderItem.DateModified;
                fd.SetFileSize(webFolderItem.SizeMean ?? 0);
            }
            return fd;
        }

        /// <summary>
        /// Gets a webfolder by name path.
        /// </summary>
        /// <param name="namePathRelative">The name path relative to the webfolder root.</param>
        /// <param name="webFolderRoot">The root webfolder where search starts.</param>
        /// <returns>The webfolder.</returns>
        private WebFolderInfo GetWebFolderByNamePath(string namePathRelative, WebFolderInfo webFolderRoot)
        {
            WebFolderInfo webFolder = GetWebFolderItemByNamePath(namePathRelative, webFolderRoot) as WebFolderInfo;
            return webFolder;
        }

        /// <summary>
        /// Gets a webfile by name path.
        /// </summary>
        /// <param name="namePathRelative">The name path relative to the webfolder root.</param>
        /// <param name="webFolderRoot">The root webfolder where search starts.</param>
        /// <returns>The webfile.</returns>
        private WebFileInfo GetWebFileByNamePath(string namePathRelative, WebFolderInfo webFolderRoot)
        {
            WebFileInfo webFile = GetWebFolderItemByNamePath(namePathRelative, webFolderRoot) as WebFileInfo;
            return webFile;
        }

        /// <summary>
        /// Gets a webfolderitem by name path.
        /// </summary>
        /// <param name="namePathRelative">The name path relative to the webfolder root.</param>
        /// <param name="webFolderRoot">The root webfolder where search starts.</param>
        /// <returns>The webfolderitem.</returns>
        private WebFolderItemInfo GetWebFolderItemByNamePath(string namePathRelative, WebFolderInfo webFolderRoot)
        {
            WebFolderItemInfo webFolderItem = null;
            if (IsRootPath(namePathRelative))
            {
                webFolderItem = webFolderRoot;
            }
            else
            {
                string[] namePathRelativeSegments = SplitNamePath(namePathRelative);

                if (webFolderRoot != null &&
                    namePathRelativeSegments != null && namePathRelativeSegments.Length > 0)
                {
                    string webFolderItemName = namePathRelativeSegments[0];
                    if (webFolderRoot.SubItems == null)
                        webFolderRoot.SubItems = GetWebFolderSubItems(webFolderRoot);

                    if (webFolderRoot.SubItems != null)
                        foreach (WebFolderItemInfo webFolderItemSub in webFolderRoot.SubItems)
                        {
                            WebFolderInfo webFolderSub = webFolderItemSub as WebFolderInfo;
                            WebFileInfo webFileSub = webFolderItemSub as WebFileInfo;
                            if (webFolderSub != null && webFolderItemName.Equals(webFolderSub.Name, StringComparison.CurrentCultureIgnoreCase) ||
                                webFileSub != null && webFolderItemName.Equals(webFileSub.FullName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (namePathRelativeSegments.Length > 1)
                                {
                                    string namePathRelativeSub = JoinNamePathSegments(
                                        namePathRelativeSegments,
                                        1,
                                        namePathRelativeSegments.Length - 1);
                                    webFolderItem = GetWebFolderItemByNamePath(
                                        namePathRelativeSub,
                                        webFolderSub);
                                }
                                else
                                {
                                    webFolderItem = webFolderItemSub;
                                }
                                break;
                            }
                        }
                }
            }
            return webFolderItem;
        }

        /// <summary>
        /// Gets a webfolderitem by path URL.
        /// </summary>
        /// <param name="pathUrlRelative">The path URL relative to the webfolder root.</param>
        /// <param name="webFolderRoot">The root webfolder where search starts.</param>
        /// <returns>The webfolderitem.</returns>
        private WebFolderItemInfo GetWebFolderItemByPathUrl(string pathUrlRelative, WebFolderInfo webFolderRoot)
        {
            WebFolderItemInfo webFolderItem = null;
            if (IsRootPath(pathUrlRelative))
            {
                webFolderItem = webFolderRoot;
            }
            else
            {
                string[] pathUrlRelativeSegments = WebFolderItemInfo.GetPathUrlSegments(pathUrlRelative);

                if (webFolderRoot != null &&
                    pathUrlRelativeSegments != null && pathUrlRelativeSegments.Length > 0)
                {
                    string webFolderItemPathUrl = JoinPathUrlSegments(pathUrlRelativeSegments, 0, 1);
                    if (webFolderRoot.SubItems == null)
                        webFolderRoot.SubItems = GetWebFolderSubItems(webFolderRoot);

                    if (webFolderRoot.SubItems != null)
                        foreach (WebFolderItemInfo webFolderItemSub in webFolderRoot.SubItems)
                        {
                            if (webFolderItemSub.PathUrl.EndsWith(webFolderItemPathUrl, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (pathUrlRelativeSegments.Length > 1)
                                {
                                    string pathUrlRelativeSub = JoinPathUrlSegments(
                                        pathUrlRelativeSegments,
                                        1,
                                        pathUrlRelativeSegments.Length - 1);
                                    webFolderItem = GetWebFolderItemByPathUrl(
                                        pathUrlRelativeSub,
                                        webFolderItemSub as WebFolderInfo);
                                }
                                else
                                {
                                    webFolderItem = webFolderItemSub;
                                }
                                break;
                            }
                        }
                }
            }
            return webFolderItem;
        }
        /// <summary>
        /// Gets a collection of sub webfolderitems located in the specified webfolder.
        /// </summary>
        /// <param name="webFolder">The webfolder of which webfolderitems to get.</param>
        /// <returns>The collection of sub webfolderitems.</returns>
        private Collection<WebFolderItemInfo> GetWebFolderSubItems(WebFolderInfo webFolder)
        {
            Collection<WebFolderItemInfo> webFolderSubItems = null;

            SkyDriveWebClient wcSkyDrive = GetSkyDriveWebClient();
            if (wcSkyDrive != null)
                try
                {
                    WebFolderItemInfo[] subWebFolderItems = IsRootPath(webFolder.PathUrl) ?
                        wcSkyDrive.ListRootWebFolderItems() :
                        wcSkyDrive.ListSubWebFolderItems(webFolder);
                    webFolderSubItems = new Collection<WebFolderItemInfo>(subWebFolderItems);
                }
                catch (Exception ex)
                {
                    SkyDriveWfxErrorForm formError = new SkyDriveWfxErrorForm(ex);
                    formError.ShowDialog();
                }

            return webFolderSubItems;
        }

        /// <summary>
        /// Marks a webfolder dirty because its sub items were changed.
        /// </summary>
        /// <param name="webFolder">The webfolder to mark dirty.</param>
        private void MarkWebFolderSubItemsChanged(WebFolderInfo webFolder)
        {
            if (webFolder != null)
                if (WebFolderSubItemsLastChanged != null)
                {
                    string[] webFolderPathUrlSegments = WebFolderItemInfo.GetPathUrlSegments(webFolder.PathUrl);
                    string[] webFolderPathUrlSegmentsLastChanged = WebFolderItemInfo.GetPathUrlSegments(WebFolderSubItemsLastChanged.PathUrl);
                    int length = Math.Min(webFolderPathUrlSegments.Length, webFolderPathUrlSegmentsLastChanged.Length);
                    int idxSegment = 0;
                    
                    for (; idxSegment < length; idxSegment++)
                        if (!webFolderPathUrlSegments[idxSegment].Equals(
                                webFolderPathUrlSegmentsLastChanged[idxSegment],
                                StringComparison.CurrentCultureIgnoreCase))
                        {
                            break;
                        }
                    
                    string webFolderParentCommonPathUrl = JoinPathUrlSegments(
                        webFolderPathUrlSegments,
                        0,
                        idxSegment);
                    WebFolderInfo webFolderParentCommon = GetWebFolderItemByPathUrl(webFolderParentCommonPathUrl, WebFolderRoot) as WebFolderInfo;
                    
                    WebFolderSubItemsLastChanged = webFolderParentCommon != null ? webFolderParentCommon : WebFolderRoot;
                }
                else
                {
                    WebFolderSubItemsLastChanged = webFolder;
                }
        }

        #endregion
    }
}
