#pragma once

struct IHookHandle
{
	virtual ~IHookHandle() noexcept {}
};
struct IHookDefinition
{
	virtual ~IHookDefinition() noexcept {}
};

class HookHandle final : public IHookHandle
{
public:
	HookHandle(HHOOK hHook) noexcept;
	~HookHandle() noexcept override;
private:
	HHOOK m_hHook;
};

class InjectionHook final : public IHookDefinition
{
public:
	InjectionHook(HMODULE hModule, HOOKPROC proc) noexcept;
	~InjectionHook() noexcept override;

	std::unique_ptr<HookHandle> Hook(int idHook, DWORD dwThreadId = 0) noexcept;
private:
	HMODULE m_hModule;
	HOOKPROC m_proc;
};
