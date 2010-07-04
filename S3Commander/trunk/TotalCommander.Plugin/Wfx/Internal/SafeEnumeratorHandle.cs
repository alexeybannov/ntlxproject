using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace TotalCommander.Plugin.Wfx.Internal
{
	class SafeEnumeratorHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public static readonly SafeEnumeratorHandle MinusOne = new SafeEnumeratorHandle(new IntPtr(-1));

		public object Enumerator
		{
			get { return IsInvalid ? null : GCHandle.FromIntPtr(handle).Target; }
		}


		public SafeEnumeratorHandle(IntPtr ptr)
			: base(true)
		{
			SetHandle(ptr);
		}

		public SafeEnumeratorHandle(object enumerator)
			: base(true)
		{
			SetHandle(enumerator != null ?
				GCHandle.ToIntPtr(GCHandle.Alloc(enumerator, GCHandleType.Pinned)) :
				IntPtr.Zero
			);
		}

		protected override bool ReleaseHandle()
		{
			if (!IsInvalid)
			{
				GCHandle.FromIntPtr(handle).Free();
			}
			return true;
		}


		public static implicit operator IntPtr(SafeEnumeratorHandle handle)
		{
			return handle.handle;
		}

		public static implicit operator SafeEnumeratorHandle(IntPtr ptr)
		{
			return new SafeEnumeratorHandle(ptr);
		}
	}
}
