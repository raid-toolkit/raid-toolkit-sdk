#include "pch.h"
#include "il2cpp.h"
#include "il2cpp-object-internals.h"
#include "Time.h"
#include "Il2CppClassHolder.h"
#include "GameAssembly.h"

void Time::SetTimeScale(GameAssembly& gasm, float fTimeScale)
{
	static Il2CppClassHolder TimeKlass{ GameAssembly::Instance().FindClass("UnityEngine", "Time") };
	void* pException{};

	void* pArgs[1] = { &fTimeScale };
	gasm.il2cpp_runtime_invoke(
		TimeKlass.FindMethod("set_timeScale", 1),
		nullptr,
		pArgs,
		&pException);
}