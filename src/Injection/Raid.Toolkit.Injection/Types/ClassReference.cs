using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct ClassReference
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szNamespace;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szName;
	}
}
