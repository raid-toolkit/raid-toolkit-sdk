#pragma once

// il2cpp-object-internals.h
typedef uint16_t Il2CppChar;
#define IL2CPP_ZERO_LEN_ARRAY 0

typedef struct Il2CppString
{
    Il2CppObject object;
    int32_t length;                             ///< Length of string *excluding* the trailing null (which is included in 'chars').
    Il2CppChar* chars;
} Il2CppString;

typedef struct Il2CppReflectionType
{
    Il2CppObject object;
    const Il2CppType* type;
} Il2CppReflectionType;

typedef struct Il2CppReflectionMethod
{
    Il2CppObject object;
    const MethodInfo* method;
    Il2CppString* name;
    void* reftype;
} Il2CppReflectionMethod;