#include "pch.h"
#include "Shared.h"
#include "il2cpp.h"

#include <tlhelp32.h>

const Il2CppClass* FindClass(HMODULE hModuleGasm, const std::string& szNamespace, const std::string& szName)
{
    Method<Il2CppDomain*> il2cpp_domain_get{ hModuleGasm, "il2cpp_domain_get" };
    Method<Il2CppAssembly**, Il2CppDomain*, size_t*> il2cpp_domain_get_assemblies{ hModuleGasm, "il2cpp_domain_get_assemblies" };
    Method<Il2CppImage*, const Il2CppAssembly*> il2cpp_assembly_get_image{ hModuleGasm, "il2cpp_assembly_get_image" };
    Method<size_t, const Il2CppImage*> il2cpp_image_get_class_count{ hModuleGasm, "il2cpp_image_get_class_count" };
    Method<const Il2CppClass*, const Il2CppImage*, size_t> il2cpp_image_get_class{ hModuleGasm, "il2cpp_image_get_class" };
    Method<const MethodInfo*, Il2CppClass*, const char*, int> il2cpp_class_get_method_from_name{ hModuleGasm, "il2cpp_class_get_method_from_name" };
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
                return pClass;
            }
        }
    }
    return nullptr;
}

void InjectSelfToProcessById(DWORD processId)
{
    std::wstring wzModulePath{ GetPathOfThisModule() };
    HANDLE hProcess{ OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, processId) };
    LPTHREAD_START_ROUTINE loadLibrary{ reinterpret_cast<LPTHREAD_START_ROUTINE>(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW")) };
    size_t cb{ wzModulePath.size()*2 + 1 };
    LPVOID dllNameAddress = VirtualAllocEx(hProcess, 0, cb, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    size_t cbWritten{ 0 };
    WriteProcessMemory(hProcess, dllNameAddress, wzModulePath.c_str(), cb, &cbWritten);
    CreateRemoteThread(hProcess, 0, 0, loadLibrary, dllNameAddress, 0, 0);
    CloseHandle(hProcess);
}

std::wstring GetPathOfThisModule()
{
    wchar_t path[MAX_PATH];
    path[0] = '\0';
    HMODULE hm{};

    if (GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS |
        GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
        static_cast<LPCWSTR>(static_cast<void*>(&GetPathOfThisModule)), &hm) == 0)
    {
        return {};
    }
    if (GetModuleFileName(hm, path, MAX_PATH) == 0)
    {
        return {};
    }
    return path;
}

char* GetBaseAddressOfModule(const std::wstring& wzName)
{
    PROCESSENTRY32 entry{};
    entry.dwSize = sizeof(PROCESSENTRY32);
    char* address{ 0 };
    HANDLE snapshot{ CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS | TH32CS_SNAPMODULE32 | TH32CS_SNAPMODULE, NULL) };
    if (Process32First(snapshot, &entry) == FALSE)
        return 0;

    while (Process32Next(snapshot, &entry) == TRUE)
    {
        if (entry.th32ProcessID == GetCurrentProcessId())
        {
            MODULEENTRY32 modEntry{};
            modEntry.dwSize = sizeof(MODULEENTRY32);
            if (Module32First(snapshot, &modEntry) == FALSE)
                goto Done;
            while (Module32Next(snapshot, &modEntry) == TRUE)
            {
                if (_wcsicmp(modEntry.szModule, wzName.c_str()) == 0)
                {
                    address = reinterpret_cast<char*>(modEntry.modBaseAddr);
                    goto Done;
                }
            }
            break;
        }
    }

Done:
    if (snapshot)
        CloseHandle(snapshot);

    return address;
}

DWORD FindProcessByName(const std::wstring& wzName)
{
    PROCESSENTRY32 entry{};
    entry.dwSize = sizeof(PROCESSENTRY32);

    HANDLE snapshot{ CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL) };

    if (Process32First(snapshot, &entry) == TRUE)
    {
        while (Process32Next(snapshot, &entry) == TRUE)
        {
            if (_wcsicmp(entry.szExeFile, wzName.c_str()) == 0)
            {
                CloseHandle(snapshot);
                return entry.th32ProcessID;
            }
        }
    }
    CloseHandle(snapshot);
    return 0;
}


//
//static DWORD WINAPI launcher(void* h)
//{
//    HRSRC res = ::FindResourceA(static_cast<HMODULE>(h),
//        MAKEINTRESOURCEA(IDR_DLLENCLOSED), "DLL");
//    if (res)
//    {
//        HGLOBAL dat = ::LoadResource(static_cast<HMODULE>(h), res);
//        if (dat)
//        {
//            unsigned char* dll =
//                static_cast<unsigned char*>(::LockResource(dat));
//            if (dll)
//            {
//                size_t len = SizeofResource(static_cast<HMODULE>(h), res);
//                LaunchDll(dll, len, "MyNamespace.MyClass", "DllMain");
//            }
//        }
//    }
//    return 0;
//}
