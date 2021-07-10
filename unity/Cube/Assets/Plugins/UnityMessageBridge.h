#include <functional>

typedef void (*CSharpDelegateCallBack)(const wchar_t* str);

extern "C" _declspec(dllexport) void __stdcall onUnityMessage(wchar_t* message);
extern "C" _declspec(dllexport) void __stdcall SendMessageToUnity(const wchar_t* message);
extern "C" _declspec(dllexport) void __stdcall InitCSharpDelegate(CSharpDelegateCallBack callback);
extern "C" _declspec(dllexport) void __stdcall AddUnityMessageCallBack(std::function<void(wchar_t*)> callback);