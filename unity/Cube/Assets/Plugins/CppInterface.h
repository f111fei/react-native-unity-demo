typedef void (*CallBack)(wchar_t* str);

CallBack G_CallBack;

extern "C" __declspec(dllexport) int __stdcall CountLettersInString(wchar_t* str);
extern "C" _declspec(dllexport) void __stdcall onUnityMessage(wchar_t* str);
extern "C" _declspec(dllexport) void __stdcall InitCSharpDelegate(CallBack callback);