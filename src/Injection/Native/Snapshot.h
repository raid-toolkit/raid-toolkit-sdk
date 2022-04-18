#pragma once
#include <string>
#include <tlhelp32.h>

class Snapshot
{
public:
	Snapshot() noexcept;
	~Snapshot() noexcept;

	bool FindProcess(DWORD procId) noexcept;
	bool FindProcess(const std::wstring& wzName) noexcept;
	bool FindFirstThread() noexcept;
	bool NextProcess() noexcept;
	bool NextModule() noexcept;
	bool NextThread() noexcept;

	const PROCESSENTRY32& Process() const noexcept
	{
		return m_process;
	}
	const MODULEENTRY32& Module() const noexcept
	{
		return m_module;
	}
	const THREADENTRY32& Thread() const noexcept
	{
		return m_thread;
	}
private:
	HANDLE m_snapshot;
	PROCESSENTRY32 m_process{};
	MODULEENTRY32 m_module{};
	THREADENTRY32 m_thread{};
};
