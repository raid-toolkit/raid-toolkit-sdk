#include "pch.h"
#include "GameAssembly.h"
#include "Time.h"
#include "MessageHandler.h"
#include "Il2CppClassHolder.h"
#include "GameAssembly.h"
#include "SystemString.h"
#include <vector>

void CallInstMethodCore(GameAssembly& gasm, const CallMethodMessage& call, void* pInst) noexcept
{
	Il2CppClassHolder klass{ GameAssembly::Instance().FindClass(call.cls.szNamespace, call.cls.szName) };
	if (klass.Empty())
		return;

	MethodInfo* pMethodInfo{ klass.FindMethod(call.fn.szName, call.fn.cParam) };
	if (!pMethodInfo)
		return;

	void* pException{};
	void** pArgs{ reinterpret_cast<void**>(malloc(sizeof(uintptr_t) * call.fn.cParam)) };
	if (!pArgs)
		return;

	std::vector<SystemString> strings{};
	// TODO: Arrays are hardcoded and tend to explode
	std::vector<System_Int32_array> arrayi32s{};
	std::vector<Il2CppArrayBounds> bounds{};
	for (int n{ 0 }; n < call.fn.cParam; ++n)
	{
		switch (call.args[n].type)
		{
		case ArgumentValueType::Number:
			pArgs[n] = const_cast<void*>(reinterpret_cast<const void*>(&call.args[n].Number.u64));
			break;
		case ArgumentValueType::String:
			strings.emplace_back(call.args[n].wzString);
			pArgs[n] = &strings.back();
			break;
		// TODO: Arrays are hardcoded and tend to explode
		case ArgumentValueType::Array:
		{
			auto valueArray = call.args[n].ValueArray;
			if (valueArray.cbValue == sizeof(int32_t))
			{
				arrayi32s.emplace_back();
				System_Int32_array& ar = arrayi32s.back();
				bounds.emplace_back();
				ar.bounds = &bounds.back();
				ar.bounds->length = valueArray.cValue;
				ar.bounds->lower_bound = 0;
				ar.max_length = valueArray.cValue;
				for (uint32_t p{ 0 }; p < valueArray.cValue; ++p)
					ar.m_Items[p] = static_cast<int32_t>(valueArray.values.i64[p]);
				
				pArgs[n] = &arrayi32s.back();
			}
			break;
		}
		}
	}
	gasm.il2cpp_runtime_invoke(
		pMethodInfo,
		pInst,
		reinterpret_cast<void**>(pArgs),
		&pException);
}

void CallStaticMethod(GameAssembly& gasm, const CallStaticMethodMessage& msg) noexcept
{
	const CallMethodMessage& call{ msg.call };
	CallInstMethodCore(gasm, call, nullptr);
}

void CallInstMethod(GameAssembly& gasm, const CallInstanceMethodMessage& msg) noexcept
{
	const CallMethodMessage& call{ msg.call };
	CallInstMethodCore(gasm, call, msg.pInstance);
}

bool HandleCopyData(const COPYDATASTRUCT* lpccds)
{
	static GameAssembly& gasm{ GameAssembly::Initialize(GetModuleHandleA("gameassembly.dll")) };

	switch (static_cast<InjectedMessageType>(lpccds->dwData))
	{
	case InjectedMessageType::Ping: {
		return true;
	}
	case InjectedMessageType::CallStaticMethod: {
		CallStaticMethod(gasm, *reinterpret_cast<CallStaticMethodMessage*>(lpccds->lpData));
		return true;
	}
	case InjectedMessageType::CallInstanceMethod: {
		CallInstMethod(gasm, *reinterpret_cast<CallInstanceMethodMessage*>(lpccds->lpData));
		return true;
	}
	default:
		break;
	}
	return false;
}

extern "C"
__declspec(dllexport) LRESULT HandleHookedMessage(int code, WPARAM wParam, LPARAM lParam)
{
	const CWPSTRUCT* pMsg{ reinterpret_cast<CWPSTRUCT*>(lParam) };
	if (pMsg)
	{
		switch (pMsg->message) {
		case WM_COPYDATA:
			if (HandleCopyData(reinterpret_cast<COPYDATASTRUCT*>(pMsg->lParam)))
				return CallNextHookEx(NULL, code, wParam, lParam);
			break;
		default:
			break;
		}
	}

	return CallNextHookEx(NULL, code, wParam, lParam);
}
