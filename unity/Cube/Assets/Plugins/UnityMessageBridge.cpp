#include "UnityMessageBridge.h"

static CSharpDelegateCallBack sendMessageCallBack;

static std::function<void(wchar_t *)> unityMessageCallBack;

extern "C" _declspec(dllexport) void __stdcall AddUnityMessageCallBack(std::function<void(wchar_t*)> callback)
{
    unityMessageCallBack = callback;
}

extern "C" _declspec(dllexport) void __stdcall onUnityMessage(wchar_t* message)
{
    if (unityMessageCallBack)
    {
        unityMessageCallBack(message);
    }
    else
    {
        wprintf(L"WARNING: Recived message: %ls, but unityMessageCallBackFunction is not setted.", message);
    }
}

extern "C" _declspec(dllexport) void __stdcall InitCSharpDelegate(CSharpDelegateCallBack callback)
{
    sendMessageCallBack = callback;
}

extern "C" _declspec(dllexport) void __stdcall SendMessageToUnity(const wchar_t* message)
{
    if (sendMessageCallBack)
    {
        sendMessageCallBack(message);
    }
}