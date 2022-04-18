#pragma once

enum class InjectedMessageType : uint8_t
{
	Ping = 1,
	CallStaticMethod,
	CallInstanceMethod,
};

constexpr size_t MaxNameLength{ 128 };
constexpr size_t MaxArrayLength{ 16 };

union NumberValue {
	float f;
	double d;
	uint64_t u64;
	int64_t i64;
};

union NumberValueArray {
	float f[MaxArrayLength];
	double d[MaxArrayLength];
	uint64_t u64[MaxArrayLength];
	int64_t i64[MaxArrayLength];
};

struct ClassReference
{
	char szNamespace[MaxNameLength];
	char szName[MaxNameLength];
};

struct MethodReference
{
	char szName[MaxNameLength];
	int32_t cParam;
};

enum class ArgumentValueType : uint32_t
{
	None = 0,
	Number = 1,
	String = 2,
	Array = 3,
};

struct ArrayValue
{
	size_t cbValue;
	uint32_t cValue;
	NumberValueArray values;
};

struct ArgumentValue
{
	ArgumentValueType type;
	uint32_t _unused;
	union {
		NumberValue Number;
		wchar_t wzString[MaxNameLength];
		ArrayValue ValueArray;
	};
};


struct CallMethodMessage
{
	ClassReference cls;
	MethodReference fn;
	ArgumentValue args[MaxArrayLength];
};

struct CallInstanceMethodMessage
{
	void* pInstance;
	CallMethodMessage call;
};

struct CallStaticMethodMessage
{
	CallMethodMessage call;
};
