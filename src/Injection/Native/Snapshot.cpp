#include "pch.h"
#include "Snapshot.h"

Snapshot::Snapshot() noexcept
{
	m_snapshot = CreateToolhelp32Snapshot(
		TH32CS_SNAPPROCESS |
		TH32CS_SNAPMODULE32 |
		TH32CS_SNAPMODULE |
		TH32CS_SNAPTHREAD, NULL);
}

Snapshot::~Snapshot() noexcept
{
	CloseHandle(m_snapshot);
}

bool Snapshot::FindProcess(DWORD procId) noexcept
{
	while (NextProcess())
	{
		if (m_process.th32ProcessID == procId)
		{
			return true;
		}
	}
	return false;
}

bool Snapshot::FindProcess(const std::wstring& wzName) noexcept
{
	while (NextProcess())
	{
		if (_wcsicmp(m_process.szExeFile, wzName.c_str()) == 0)
		{
			return true;
		}
	}
	return false;
}

bool Snapshot::FindFirstThread() noexcept
{
	m_thread = {};
	while (NextThread())
	{
		if (m_thread.th32OwnerProcessID == m_process.th32ProcessID)
			return true;
	}
	m_thread = {};
	return false;
}

bool Snapshot::NextProcess() noexcept
{
	m_module = {};
	m_thread = {};
	if (m_process.dwSize == 0)
	{
		m_process.dwSize = sizeof(PROCESSENTRY32);
		return !!Process32First(m_snapshot, &m_process);
	}
	else
	{
		m_process.dwSize = sizeof(PROCESSENTRY32);
		return !!Process32Next(m_snapshot, &m_process);
	}
}

bool Snapshot::NextModule() noexcept
{
	if (m_module.dwSize == 0)
	{
		m_module.dwSize = sizeof(MODULEENTRY32);
		return !!Module32First(m_snapshot, &m_module);
	}
	else
	{
		m_module.dwSize = sizeof(MODULEENTRY32);
		return !!Module32Next(m_snapshot, &m_module);
	}
}


bool Snapshot::NextThread() noexcept
{
	if (m_thread.dwSize == 0)
	{
		m_thread.dwSize = sizeof(THREADENTRY32);
		return !!Thread32First(m_snapshot, &m_thread);
	}
	else
	{
		m_thread.dwSize = sizeof(THREADENTRY32);
		return !!Thread32Next(m_snapshot, &m_thread);
	}
}