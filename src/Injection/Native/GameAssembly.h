#pragma once
#include "Shared.h"
struct MethodInfo;
struct Il2CppObject;
struct Il2CppType;
struct Il2CppDomain;
struct Il2CppImage;
struct Il2CppAssembly;
struct Il2CppClass;
class Il2CppClassHolder;

class GameAssembly {
private:
	GameAssembly(HMODULE hModuleGasm)
		: hModule{ hModuleGasm }
		, il2cpp_class_get_methods{ hModuleGasm, "il2cpp_class_get_methods" }
		, il2cpp_class_get_method_from_name{ hModuleGasm, "il2cpp_class_get_method_from_name" }
		, il2cpp_runtime_invoke{ hModuleGasm, "il2cpp_runtime_invoke" }
		, il2cpp_type_get_type{ hModuleGasm, "il2cpp_type_get_type" }
		, il2cpp_alloc{ hModuleGasm, "il2cpp_alloc" }
		, il2cpp_free{ hModuleGasm, "il2cpp_free" }
		, il2cpp_domain_get{ hModuleGasm, "il2cpp_domain_get" }
		, il2cpp_domain_get_assemblies{ hModuleGasm, "il2cpp_domain_get_assemblies" }
		, il2cpp_assembly_get_image{ hModuleGasm, "il2cpp_assembly_get_image" }
		, il2cpp_image_get_class_count{ hModuleGasm, "il2cpp_image_get_class_count" }
		, il2cpp_image_get_class{ hModuleGasm, "il2cpp_image_get_class" }
	{}

public:
	static GameAssembly& Initialize(HMODULE hModule) noexcept;
	static GameAssembly& Instance() noexcept;

	Il2CppClassHolder FindClass(const std::string& szNamespace, const std::string& szName) noexcept;

	HMODULE hModule;
	Method<const MethodInfo*, Il2CppClass*, void**> il2cpp_class_get_methods;
	Method<const MethodInfo*, Il2CppClass*, const char*, int> il2cpp_class_get_method_from_name;
	Method<Il2CppObject*, const MethodInfo* /*pmethod*/, void* /*pinst*/, void** /*pargs*/, void** /*pex*/> il2cpp_runtime_invoke;
	Method<int, const Il2CppType*> il2cpp_type_get_type;
	Method<void*, size_t> il2cpp_alloc;
	Method<void, void*> il2cpp_free;
	Method<Il2CppDomain*> il2cpp_domain_get;
	Method<Il2CppAssembly**, Il2CppDomain*, size_t*> il2cpp_domain_get_assemblies;
	Method<Il2CppImage*, const Il2CppAssembly*> il2cpp_assembly_get_image;
	Method<size_t, const Il2CppImage*> il2cpp_image_get_class_count;
	Method<const Il2CppClass*, const Il2CppImage*, size_t> il2cpp_image_get_class;
};
