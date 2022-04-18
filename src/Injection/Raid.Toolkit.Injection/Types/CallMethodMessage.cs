using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential)]
	public struct CallMethodMessage
	{
		public ClassReference cls;
		public MethodReference fn;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public ArgumentValue[] args;
	};
}
