using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.System.JobObjects;

namespace Raid.Toolkit;

public class Job : IDisposable
{
	private SafeHandle handle;
	private bool disposed;

	public Job()
	{
		handle = PInvoke.CreateJobObject(null, (string?)null);

		JOBOBJECT_BASIC_LIMIT_INFORMATION info = new() { LimitFlags = JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE };
		JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new() { BasicLimitInformation = info };

		int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
		IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
		Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

		unsafe
		{
			if (!PInvoke.SetInformationJobObject(handle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, (void*)extendedInfoPtr, (uint)length))
				throw new Exception($"Unable to set information.  Error: {Marshal.GetLastWin32Error()}");
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposed)
			return;

		if (disposing) { }

		Close();
		disposed = true;
	}

	public void Close()
	{
		handle.Close();
	}

	public bool AddProcess(SafeProcessHandle processHandle)
	{
		return PInvoke.AssignProcessToJobObject(handle, processHandle);
	}

	public bool AddProcess(Process process)
	{
		return AddProcess(process.SafeHandle);
	}
}
