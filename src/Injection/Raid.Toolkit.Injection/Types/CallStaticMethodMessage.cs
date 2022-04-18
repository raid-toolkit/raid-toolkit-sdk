using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    [StructLayout(LayoutKind.Sequential)]
	public struct CallStaticMethodMessage
	{
		public CallMethodMessage call;
	};
}
