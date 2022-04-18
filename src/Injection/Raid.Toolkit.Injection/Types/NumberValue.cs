using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Explicit)]
	public struct NumberValue
	{
		[FieldOffset(0)]
		public float f;
		[FieldOffset(0)]
		public double d;
		[FieldOffset(0)]
		public ulong u64;
		[FieldOffset(0)]
		public long i64;
	}
}
