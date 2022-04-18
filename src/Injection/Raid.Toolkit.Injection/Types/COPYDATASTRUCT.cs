using System;
using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential)]
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;
		public int cbData;
		public IntPtr lpData;
	}
}
