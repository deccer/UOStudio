using System.Collections.Generic;

namespace UOStudio.Client.Launcher.ViewModels
{
    public interface INavigationAware
    {
        bool IsNavigationTarget();

        void OnNavigatedTo(IDictionary<string, object> navigationParameters);
    }
}