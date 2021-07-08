#pragma once

#include "App.xaml.g.h"

namespace activation = winrt::Windows::ApplicationModel::Activation;

namespace winrt::rnunitydemo::implementation
{
    struct App : AppT<App>
    {
        App() noexcept;
        void OnLaunched(activation::LaunchActivatedEventArgs const&);
        void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs const& e);
        void OnSuspending(IInspectable const&, Windows::ApplicationModel::SuspendingEventArgs const&);
        void OnNavigationFailed(IInspectable const&, Windows::UI::Xaml::Navigation::NavigationFailedEventArgs const&);
    private:
        using super = AppT<App>;
        UnityPlayer::AppCallbacks m_AppCallbacks;
        //activation::SplashScreen m_SplashScreen;
        void SetupOrientation();
        void InitializeUnity(hstring args);
    };
} // namespace winrt::uwp::implementation
