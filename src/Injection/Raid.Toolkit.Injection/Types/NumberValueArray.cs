using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Explicit)]
	public struct NumberValueArray
	{
		[FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public float[] f;
		[FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public double[] d;
		[FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public ulong[] u64;
		[FieldOffset(0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public long[] i64;
	}
}
