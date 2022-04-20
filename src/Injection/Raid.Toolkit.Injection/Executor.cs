using System;
using System.Runtime.InteropServices;

namespace Raid.Toolkit.Injection
{
	internal class Executor
	{
		public static void InvokeInstanceFunction(IntPtr hwnd, ulong instancePtr, CallMethodMessage call)
		{
			CallInstanceMethodMessage msg = new() { call = call, pInstance = (IntPtr)instancePtr };

			IntPtr msgPtr = Interop.IntPtrAlloc(msg);
			try
			{
				SendMessage(hwnd, Marshal.SizeOf(msg), msgPtr, InjectedMessageType.CallInstanceMethod);
			}
			finally
			{
				Interop.IntPtrFree(ref msgPtr);
			}
		}

		public static void InvokeStaticFunction(IntPtr hwnd, CallMethodMessage call)
		{
			CallStaticMethodMessage msg = new() { call = call };

			IntPtr msgPtr = Interop.IntPtrAlloc(msg);
			try
			{
                SendMessage(hwnd, Marshal.SizeOf(msg), msgPtr, InjectedMessageType.CallStaticMethod);
			}
			finally
			{
				Interop.IntPtrFree(ref msgPtr);
			}
		}

		private static void SendMessage(IntPtr hwnd, int cbMsg, IntPtr pMsg, InjectedMessageType type)
		{
			COPYDATASTRUCT cds = new()
			{
				dwData = (IntPtr)type,
				cbData = cbMsg,
				lpData = pMsg
			};
			IntPtr cdsPtr = Interop.IntPtrAlloc(cds);
			try
			{
				Interop.SendMessage(hwnd, 0x004A /* WM_SENDDATA */, UIntPtr.Zero, cdsPtr);
			}
			finally
			{
				Interop.IntPtrFree(ref cdsPtr);
			}
		}
	}
}
