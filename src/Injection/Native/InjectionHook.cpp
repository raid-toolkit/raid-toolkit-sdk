#include "pch.h"
#include "InjectionHook.h"

InjectionHook::InjectionHook(HMODULE hModule, HOOKPROC proc) noexcept
	: m_hModule{ hModule }
	, m_proc{ proc }
{
}

InjectionHook::~InjectionHook() noexcept
{
}

std::unique_ptr<HookHandle> InjectionHook::Hook(int idHook, DWORD dwThreadId) noexcept
{
	HHOOK hHook{ SetWindowsHookEx(idHook, m_proc, m_hModule, dwThreadId) };
	return std::make_unique<HookHandle>(hHook);
}

HookHandle::HookHandle(HHOOK hHook) noexcept
	: m_hHook{ hHook }
{}

HookHandle::~HookHandle() noexcept
{
	if (m_hHook)
	{
		UnhookWindowsHookEx(m_hHook);
	}
}
