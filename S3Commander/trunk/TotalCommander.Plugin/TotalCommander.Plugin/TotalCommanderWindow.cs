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


        public void Refresh()
        {
            Win32.PostMessage(Handle, 1024 + 51, (IntPtr)540, IntPtr.Zero);
        }
    }
}
