using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Explicit)]
	public struct ArgumentValue
	{
		[FieldOffset(0)]
		public uint type;

		[FieldOffset(4)]
		public uint _unused;

		[FieldOffset(8)]
		public NumberValue Number;
		//[FieldOffset(8), MarshalAs(UnmanagedType.LPStr, SizeConst = 128)]
		//public string wzString;
		[FieldOffset(8)]
		public ArrayValue ValueArray;
	}
}
