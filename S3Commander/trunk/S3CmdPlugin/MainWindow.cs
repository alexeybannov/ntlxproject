using System;
using System.Runtime.InteropServices;

namespace S3CmdPlugin
{
	class MainWindow
	{
		private const int WM_USER = 1024;
        private const int WM_REFRESH = WM_USER + 51;

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32", SetLastError = true)]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		private IntPtr handle;

		public MainWindow(IntPtr handle)
		{
			this.handle = handle;
		}

		public void Refresh()
		{
            PostMessage(handle, WM_REFRESH, (IntPtr)540, IntPtr.Zero);
		}
	}
}
