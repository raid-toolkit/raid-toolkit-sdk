#include "pch.h"
#include "WindowHelpers.h"

BOOL CALLBACK EnumWindowCallback(HWND handle, LPARAM lParam) noexcept
{
	FindWindowData& data{ *reinterpret_cast<FindWindowData*>(lParam) };
	DWORD dwProcId;
	GetWindowThreadProcessId(handle, &dwProcId);
	if (data.procId != dwProcId || (GetWindow(handle, GW_OWNER) == (HWND)0 && IsWindowVisible(handle)))
		return TRUE;

	data.hwnd = handle;
	return FALSE;
}

HWND GetMainWindowForProcessId(DWORD dwProcId) noexcept
{
	FindWindowData findHwnd{ dwProcId, 0 };
	if (EnumWindows(EnumWindowCallback, reinterpret_cast<LPARAM>(&findHwnd)) != FALSE)
	{
		return 0;
	}
	return findHwnd.hwnd;
}