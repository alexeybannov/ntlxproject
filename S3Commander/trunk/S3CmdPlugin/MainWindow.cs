using System;
using System.Runtime.InteropServices;

namespace S3CmdPlugin
{
	class MainWindow
	{
		private const int WM_USER = 1024;

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32", SetLastError = true)]
		private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		private IntPtr handle;

		public MainWindow(IntPtr handle)
		{
			this.handle = handle;
		}

		public void Refresh()
		{
			PostMessage(handle, WM_USER + 51, (IntPtr)540, IntPtr.Zero);
		}
	}
}
