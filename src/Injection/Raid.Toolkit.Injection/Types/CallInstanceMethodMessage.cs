using System;
using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential)]
	public struct CallInstanceMethodMessage
	{
		public IntPtr pInstance;
		public CallMethodMessage call;
	};
}
