using System;

namespace TotalCommander.Plugin
{
	public class MainWindow
	{
		public IntPtr Handle
		{
			get;
			private set;
		}

		public MainWindow(IntPtr handle)
		{
			Handle = handle;
		}
	}
}
