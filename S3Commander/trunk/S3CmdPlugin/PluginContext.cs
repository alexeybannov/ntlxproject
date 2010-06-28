using System;
using System.Runtime.InteropServices;

namespace S3CmdPlugin
{
	class PluginContext
	{
		[DllImport("user32", SetLastError = true)]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


		private IntPtr handle;


		public void RefreshMainWindow()
		{
			PostMessage(handle, 1024 + 51, (IntPtr)540, IntPtr.Zero);
		}

		internal void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}
	}
}
