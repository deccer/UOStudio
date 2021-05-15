using System;
using System.Collections.Generic;
using UOStudio.Client.Launcher.ViewModels;

namespace UOStudio.Client.Launcher.Services
{
    public interface INavigator
    {
        void Navigate(string uri, IDictionary<string, object> navigationParameters = null);

        event Action<string, BindableBase> OnNavigated;
    }
}