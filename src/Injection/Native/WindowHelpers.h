#pragma once
#include <windows.h>

struct FindWindowData
{
	DWORD procId;
	HWND hwnd;
};

BOOL CALLBACK EnumWindowCallback(HWND handle, LPARAM lParam) noexcept;
HWND GetMainWindowForProcessId(DWORD dwProcId) noexcept;