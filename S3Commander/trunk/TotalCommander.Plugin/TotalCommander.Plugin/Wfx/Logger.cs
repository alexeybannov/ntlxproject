using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
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

		public void Connect(string message)
		{
			Log(MessageType.Connect, message);
		}
		
		public void Disconnect(string message)
		{
			Log(MessageType.Disconnect, message);
		}

		public void Details(string message)
		{
			Log(MessageType.Details, message);
		}

		public void TransferComplete(string message)
		{
			Log(MessageType.TransferComplete, message);
		}

		public void ConnectComplete(string message)
		{
			Log(MessageType.ConnectComplete, message);
		}

		public void ImportantError(string message)
		{
			Log(MessageType.ImportantError, message);
		}

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
