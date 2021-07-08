#include "pch.h"
#include "MainPage.h"
#if __has_include("MainPage.g.cpp")
#include "MainPage.g.cpp"
#endif

#include "App.h"

using namespace winrt;
using namespace Windows::UI::Xaml;

using namespace UnityPlayer;

namespace winrt::rnunitydemo::implementation
{
    MainPage::MainPage()
    {
        InitializeComponent();
        auto app = Application::Current().as<App>();
        ReactRootView().ReactNativeHost(app->Host());

        auto appCallbacks = AppCallbacks::Instance();

        appCallbacks.SetSwapChainPanel(m_DXSwapChainPanel());
        appCallbacks.SetCoreWindowEvents(Window::Current().CoreWindow());
        appCallbacks.InitializeD3DXAML();
    }
}
