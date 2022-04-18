#pragma once
typedef int32_t TypeDefinitionIndex;
typedef int32_t CustomAttributeIndex;
typedef int32_t MethodIndex;

struct Il2CppAssembly;

typedef struct Il2CppDomain
{
    void* domain;
    void* setup;
    void* default_context;
    const char* friendly_name;
    uint32_t domain_id;

#if NET_4_0
    volatile int threadpool_jobs;
#endif
    void* agent_info;
} Il2CppDomain;
typedef struct Il2CppAssemblyName
{
    const char* name;
    const char* culture;
    const char* hash_value;
    const char* public_key;
    uint32_t hash_alg;
    int32_t hash_len;
    uint32_t flags;
    int32_t major;
    int32_t minor;
    int32_t build;
    int32_t revision;
    uint8_t public_key_token[64]; //PUBLIC_KEY_BYTE_LENGTH
} Il2CppAssemblyName;

typedef struct Il2CppImage
{
    const char* name;
    const char* nameNoExt;
    Il2CppAssembly* assembly;

    TypeDefinitionIndex typeStart;
    uint32_t typeCount;

    TypeDefinitionIndex exportedTypeStart;
    uint32_t exportedTypeCount;

    CustomAttributeIndex customAttributeStart;
    uint32_t customAttributeCount;

    MethodIndex entryPointIndex;

#ifdef __cplusplus
    mutable
#endif
    void* nameToClassHashTable;

    uint32_t token;
    uint8_t dynamic;
} Il2CppImage;

typedef struct Il2CppAssembly
{
    Il2CppImage* image;
    uint32_t token;
    int32_t referencedAssemblyStart;
    int32_t referencedAssemblyCount;
    Il2CppAssemblyName aname;
} Il2CppAssembly;

/*
typedef void(*Il2CppMethodPointer)();

struct MethodInfo;

struct VirtualInvokeData
{
    Il2CppMethodPointer methodPtr;
    const MethodInfo* method;
};

struct Il2CppType
{
    void* data;
    unsigned int bits;
};

struct Il2CppClass;

struct Il2CppObject
{
    Il2CppClass* klass;
    void* monitor;
};

union Il2CppRGCTXData
{
    void* rgctxDataDummy;
    const MethodInfo* method;
    const Il2CppType* type;
    Il2CppClass* klass;
};

struct Il2CppRuntimeInterfaceOffsetPair
{
    Il2CppClass* interfaceType;
    int32_t offset;
};
struct Il2CppClass_1
{
    void* image;
    void* gc_desc;
    const char* name;
    const char* namespaze;
    Il2CppType byval_arg;
    Il2CppType this_arg;
    Il2CppClass* element_class;
    Il2CppClass* castClass;
    Il2CppClass* declaringType;
    Il2CppClass* parent;
    void* generic_class;
    void* typeDefinition;
    void* interopData;
    Il2CppClass* klass;
    void* fields;
    void* events;
    void* properties;
    void* methods;
    Il2CppClass** nestedTypes;
    Il2CppClass** implementedInterfaces;
    Il2CppRuntimeInterfaceOffsetPair* interfaceOffsets;
};

struct Il2CppClass_2
{
    Il2CppClass** typeHierarchy;
    uint32_t initializationExceptionGCHandle;
    uint32_t cctor_started;
    uint32_t cctor_finished;
    uint64_t cctor_thread;
    int32_t genericContainerIndex;
    uint32_t instance_size;
    uint32_t actualSize;
    uint32_t element_size;
    int32_t native_size;
    uint32_t static_fields_size;
    uint32_t thread_static_fields_size;
    int32_t thread_static_fields_offset;
    uint32_t flags;
    uint32_t token;
    uint16_t method_count;
    uint16_t property_count;
    uint16_t field_count;
    uint16_t event_count;
    uint16_t nested_type_count;
    uint16_t vtable_count;
    uint16_t interfaces_count;
    uint16_t interface_offsets_count;
    uint8_t typeHierarchyDepth;
    uint8_t genericRecursionDepth;
    uint8_t rank;
    uint8_t minimumAlignment;
    uint8_t naturalAligment;
    uint8_t packingSize;
    uint8_t bitflags1;
    uint8_t bitflags2;
};

struct Il2CppClass
{
    Il2CppClass_1 _1;
    void* static_fields;
    Il2CppRGCTXData* rgctx_data;
    Il2CppClass_2 _2;
    VirtualInvokeData vtable[255];
};

typedef uintptr_t il2cpp_array_size_t;
typedef int32_t il2cpp_array_lower_bound_t;
struct Il2CppArrayBounds
{
    il2cpp_array_size_t length;
    il2cpp_array_lower_bound_t lower_bound;
};

struct MethodInfo
{
    Il2CppMethodPointer methodPointer;
    void* invoker_method;
    const char* name;
    Il2CppClass* klass;
    const Il2CppType* return_type;
    const void* parameters;
    union
    {
        const Il2CppRGCTXData* rgctx_data;
        const void* methodDefinition;
    };
    union
    {
        const void* genericMethod;
        const void* genericContainer;
    };
    uint32_t token;
    uint16_t flags;
    uint16_t iflags;
    uint16_t slot;
    uint8_t parameters_count;
    uint8_t bitflags;
};

struct __declspec(align(8)) UnityEngine_Object_Fields {
    intptr_t m_CachedPtr;
};
struct UnityEngine_GameObject_Fields : UnityEngine_Object_Fields {
};
struct UnityEngine_GameObject_VTable {
    VirtualInvokeData _0_Equals;
    VirtualInvokeData _1_Finalize;
    VirtualInvokeData _2_GetHashCode;
    VirtualInvokeData _3_ToString;
};
struct UnityEngine_GameObject_c {
    Il2CppClass_1 _1;
    void* static_fields;
    Il2CppRGCTXData* rgctx_data;
    Il2CppClass_2 _2;
    UnityEngine_GameObject_VTable vtable;
};
struct UnityEngine_GameObject_o {
    UnityEngine_GameObject_c* klass;
    void* monitor;
    UnityEngine_GameObject_Fields fields;
};

struct __declspec(align(8)) System_String_Fields {
    int32_t m_stringLength;
    uint16_t m_firstChar;
};
struct System_String_VTable {
    VirtualInvokeData _0_Equals;
    VirtualInvokeData _1_Finalize;
    VirtualInvokeData _2_GetHashCode;
    VirtualInvokeData _3_ToString;
    VirtualInvokeData _4_CompareTo;
    VirtualInvokeData _5_Clone;
    VirtualInvokeData _6_GetTypeCode;
    VirtualInvokeData _7_System_IConvertible_ToBoolean;
    VirtualInvokeData _8_System_IConvertible_ToChar;
    VirtualInvokeData _9_System_IConvertible_ToSByte;
    VirtualInvokeData _10_System_IConvertible_ToByte;
    VirtualInvokeData _11_System_IConvertible_ToInt16;
    VirtualInvokeData _12_System_IConvertible_ToUInt16;
    VirtualInvokeData _13_System_IConvertible_ToInt32;
    VirtualInvokeData _14_System_IConvertible_ToUInt32;
    VirtualInvokeData _15_System_IConvertible_ToInt64;
    VirtualInvokeData _16_System_IConvertible_ToUInt64;
    VirtualInvokeData _17_System_IConvertible_ToSingle;
    VirtualInvokeData _18_System_IConvertible_ToDouble;
    VirtualInvokeData _19_System_IConvertible_ToDecimal;
    VirtualInvokeData _20_System_IConvertible_ToDateTime;
    VirtualInvokeData _21_ToString;
    VirtualInvokeData _22_System_IConvertible_ToType;
    VirtualInvokeData _23_System_Collections_IEnumerable_GetEnumerator;
    VirtualInvokeData _24_CompareTo;
    VirtualInvokeData _25_System_Collections_Generic_IEnumerable_System_Char__GetEnumerator;
    VirtualInvokeData _26_Equals;
};
struct System_String_c {
    Il2CppClass_1 _1;
    struct System_String_StaticFields* static_fields;
    Il2CppRGCTXData* rgctx_data;
    Il2CppClass_2 _2;
    System_String_VTable vtable;
};
struct System_String_o {
    System_String_c* klass;
    void* monitor;
    System_String_Fields fields;
};
struct System_String_StaticFields {
    struct System_String_o* Empty;
};
*/