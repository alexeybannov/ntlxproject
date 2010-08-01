using System;

namespace TotalCommander.Plugin
{
	public class TotalCommanderWindow
	{
		public IntPtr Handle
		{
			get;
			private set;
		}

		public TotalCommanderWindow(IntPtr handle)
		{
			Handle = handle;
		}
	}
}
