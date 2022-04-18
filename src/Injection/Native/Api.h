#pragma once
#include "framework.h"

enum class InjectResult : DWORD
{
	Unhooked = 0,
	Hooked = 1,
};

extern "C" __declspec(dllexport) HRESULT WINAPI InjectHook(DWORD procId);
extern "C" __declspec(dllexport) HRESULT WINAPI ReleaseHook(DWORD procId);
extern "C" __declspec(dllexport) InjectResult WINAPI GetHookState(DWORD procId);
