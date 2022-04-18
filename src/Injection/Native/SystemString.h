#pragma once
#include <string>
#include "il2cpp.h"
#include "il2cpp-object-internals.h"

struct SystemString final
{
public:
	SystemString(const std::wstring& value) noexcept;
	~SystemString() noexcept;
	System_String_o* Value() noexcept;
private:
	System_String_o* m_pStr{};
};