using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="Logger"/> is a class, which the plugin can use to show the FTP connections toolbar, and to pass log messages to it.
    /// Totalcmd can show these messages in the log window (ftp toolbar) and write them to a log file.
    /// This class is received through the <see cref="ITotalCommanderWfxPlugin.Init"/> function when the plugin is loaded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Total Commander supports logging to files. 
    /// While one log file will store all messages, the other will only store important errors, connects, disconnects and 
    /// complete operations/transfers, but not messages by <see cref="Logger.Details"/> method.
    /// </para>
    /// <para>
    /// Do NOT call <see cref="Logger.Connect"/> if your plugin does not require connect/disconnect!
    /// If you call it, the function <see cref="ITotalCommanderWfxPlugin.Disconnect"/> will be called when the user presses the Disconnect button.
    /// </para>
    /// <para>
    /// Examples:<br />
    /// - FTP requires connect/disconnect, so call LogProc with MSGTYPE_CONNECT when a connection is established.<br />
    /// - Access to local file systems (e.g. Linux EXT2) does not require connect/disconnect
    /// </para>
    /// </remarks>
	public class Logger
	{
		private int pluginNumber;

		private LogCallback log;

		internal Logger(int pluginNumber, LogCallback log)
		{
			if (log == null) throw new ArgumentNullException("log");

			this.pluginNumber = pluginNumber;
			this.log = log;
		}

        /// <summary>
        /// Connect to a file system requiring disconnect.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// The string MUST have a specific format: "CONNECT" followed by a single whitespace, then the root of the file system 
        /// which was connected, without trailing backslash. Example: CONNECT \Filesystem.
        /// </param>
		public void Connect(string message)
		{
			Log(MessageType.Connect, message);
		}
		
        /// <summary>
        /// Disconnected successfully.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void Disconnect(string message)
		{
			Log(MessageType.Disconnect, message);
		}

        /// <summary>
        /// Not so important messages like directory changing.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void Details(string message)
		{
			Log(MessageType.Details, message);
		}

        /// <summary>
        /// A file transfer was completed successfully.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void TransferComplete(string message)
		{
			Log(MessageType.TransferComplete, message);
		}

        /// <summary>
        /// Unused.
        /// </summary>
        /// <param name="message">
        /// String which should be logged. The string should contain both the source and target names, separated by an arrow " -> ",
        /// e.g. Download complete: \Filesystem\dir1\file1.txt -> c:\localdir\file1.txt
        /// </param>
        public void ConnectComplete(string message)
		{
			Log(MessageType.ConnectComplete, message);
		}

        /// <summary>
        /// An important error has occured.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void ImportantError(string message)
		{
			Log(MessageType.ImportantError, message);
		}

        /// <summary>
        /// An operation other than a file transfer has completed.
        /// </summary>
        /// <param name="message">
        /// String which should be logged.
        /// </param>
        public void OperationComplete(string message)
		{
			Log(MessageType.OperationComplete, message);
		}


		private void Log(MessageType messageType, string message)
		{
			log(pluginNumber, (int)messageType, message);
		}
	}
}
