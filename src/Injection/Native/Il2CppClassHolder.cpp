#include "pch.h"
#include "Il2CppClassHolder.h"
#include "GameAssembly.h"

Il2CppClassHolder::Il2CppClassHolder(Il2CppClass* pClass) noexcept
	: m_pClass{pClass}
{
}

bool Il2CppClassHolder::Empty() const noexcept
{
	return m_pClass == 0;
}


MethodInfo* Il2CppClassHolder::FindMethod(const std::string& name, const int num_args) noexcept
{
	return const_cast<MethodInfo*>(GameAssembly::Instance().il2cpp_class_get_method_from_name(m_pClass, name.c_str(), num_args));
}

Il2CppClass& Il2CppClassHolder::Class() noexcept
{
	return *m_pClass;
}
