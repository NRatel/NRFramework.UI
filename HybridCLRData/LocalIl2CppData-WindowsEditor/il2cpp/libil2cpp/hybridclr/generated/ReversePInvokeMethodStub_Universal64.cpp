#include "../metadata/ReversePInvokeMethodStub.h"
#include "../metadata/MetadataModule.h"

namespace hybridclr
{
namespace metadata
{
#if HYBRIDCLR_ABI_UNIVERSAL_64

	//!!!{{CODE

	void __ReversePInvokeMethod_0(int32_t __arg0, int32_t __arg1)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(0);
        typedef int32_t (*Callback)(int32_t, int32_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, __arg1, method);
	}

	void __ReversePInvokeMethod_1(int32_t __arg0, int32_t __arg1)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(1);
        typedef int32_t (*Callback)(int32_t, int32_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, __arg1, method);
	}

	void __ReversePInvokeMethod_2(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(0);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_3(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(1);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_4(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(2);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_5(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(3);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_6(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(4);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_7(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(5);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_8(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(6);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_9(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(7);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_10(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(8);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

	void __ReversePInvokeMethod_11(int64_t __arg0)
	{
        const MethodInfo* method = MetadataModule::GetMethodInfoByReversePInvokeWrapperIndex(9);
        typedef int32_t (*Callback)(int64_t, const MethodInfo*);
		((Callback)(method->methodPointerCallByInterp))(__arg0, method);
	}

    ReversePInvokeMethodData g_reversePInvokeMethodStub[]
	{
		{"i4i4i4", (Il2CppMethodPointer)__ReversePInvokeMethod_0},
		{"i4i4i4", (Il2CppMethodPointer)__ReversePInvokeMethod_1},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_2},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_3},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_4},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_5},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_6},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_7},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_8},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_9},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_10},
		{"i4i8", (Il2CppMethodPointer)__ReversePInvokeMethod_11},

		{nullptr, nullptr},
	};


	//!!!}}CODE
#endif
}
}