#include "CppInterface.h"

extern "C" __declspec(dllexport) int __stdcall CountLettersInString(wchar_t* str)
{
    int length = 0;
    while (*str++ != L'\0')
        length++;
    return length;
}

extern "C" _declspec(dllexport) void __stdcall onUnityMessage(wchar_t* str)
{
}

extern "C" _declspec(dllexport) void __stdcall InitCSharpDelegate(CallBack callback)
{
	G_CallBack = callback;
}