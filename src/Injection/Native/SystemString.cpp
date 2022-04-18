#include "pch.h"
#include "SystemString.h"
#include "Il2CppClassHolder.h"
#include "GameAssembly.h"

SystemString::SystemString(const std::wstring& value) noexcept
{
	static Il2CppClassHolder StringKlass{ GameAssembly::Instance().FindClass("System", "String") };
	size_t cb{ sizeof(System_String_o) + value.length() * 2 + 2 };
	void* strAlloc{ GameAssembly::Instance().il2cpp_alloc(cb) };
	if (!strAlloc)
		return;
	memset(strAlloc, 0, cb);
	m_pStr = reinterpret_cast<System_String_o*>(strAlloc);
	m_pStr->klass = reinterpret_cast<System_String_c*>(const_cast<Il2CppClass*>(&StringKlass.Class()));
	m_pStr->monitor = reinterpret_cast<void*>(0x0);
	m_pStr->fields.m_stringLength = static_cast<int32_t>(value.length());
	memcpy(&m_pStr->fields.m_firstChar, value.c_str(), value.length() * 2);
}

SystemString::~SystemString() noexcept
{
	GameAssembly::Instance().il2cpp_free(m_pStr);
}

System_String_o* SystemString::Value() noexcept
{
	return m_pStr;
}