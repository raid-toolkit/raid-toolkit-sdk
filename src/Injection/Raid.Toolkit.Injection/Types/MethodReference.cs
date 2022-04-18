using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct MethodReference
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szName;

		public int cParam;
	}
}
