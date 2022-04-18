#include "pch.h"
#include "GameAssembly.h"
#include "il2cpp.h"
#include "il2cpp-object-internals.h"
#include "Il2CppClassHolder.h"

GameAssembly* g_pInstance;

GameAssembly& GameAssembly::Initialize(HMODULE hModule) noexcept
{
	g_pInstance = new GameAssembly(hModule);
	return *g_pInstance;
}

GameAssembly& GameAssembly::Instance() noexcept
{
	return *g_pInstance;
}

Il2CppClassHolder GameAssembly::FindClass(const std::string& szNamespace, const std::string& szName) noexcept
{
	Il2CppDomain* pAppDomain{ il2cpp_domain_get() };
	size_t casm{};
	Il2CppAssembly** ppAssemblies{ il2cpp_domain_get_assemblies(pAppDomain, &casm) };

	for (size_t n{ 0 }; n < casm; ++n) {
		// UnityEngine.GameObject$$Find
		Il2CppAssembly* pAssembly{ *(ppAssemblies++) };
		Il2CppImage* pImage{ il2cpp_assembly_get_image(pAssembly) };
		size_t cclass{ il2cpp_image_get_class_count(pImage) };
		for (size_t iClass{ 0 }; iClass < cclass; ++iClass) {
			const Il2CppClass* pClass{ il2cpp_image_get_class(pImage, iClass) };
			if (_strcmpi(pClass->_1.namespaze, szNamespace.c_str()) == 0 && _strcmpi(pClass->_1.name, szName.c_str()) == 0)
			{
				return Il2CppClassHolder{ const_cast<Il2CppClass*>(pClass) };
			}
		}
	}
	return { 0 };
}