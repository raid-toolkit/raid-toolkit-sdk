using System;
using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    internal static class Interop
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		internal static extern int SendMessage(IntPtr hWnd, int uMsg, UIntPtr wParam, IntPtr lParam);

        internal static IntPtr IntPtrAlloc<T>(T param)
		{
			IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
			Marshal.StructureToPtr(param, retval, false);
			return retval;
		}

        internal static void IntPtrFree(ref IntPtr preAllocated)
		{
			Marshal.FreeHGlobal(preAllocated);
			preAllocated = IntPtr.Zero;
		}

        internal static T[] TArrayOfLength<T>(int length, params T[] values)
		{
			var t = new T[length];
			for (int i = 0; i < values.Length; i++)
				t[i] = values[i];
			return t;
		}
	}
}
