using Il2CppToolkit.Runtime;
using System;
using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
    internal static class Interop
	{
		[DllImport("Inject.dll")]
		public static extern int InjectHook(uint dwProcId);
		[DllImport("Inject.dll")]
		public static extern int ReleaseHook(uint dwProcId);
		[DllImport("Inject.dll")]
		public static extern uint GetHookState(uint dwProcId);
    }
}
