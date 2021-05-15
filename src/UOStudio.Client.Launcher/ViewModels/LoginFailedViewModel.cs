using System.Collections.Generic;
using System.Windows.Input;
using UOStudio.Client.Launcher.Commands;
using UOStudio.Client.Launcher.Services;

namespace UOStudio.Client.Launcher.ViewModels
{
    public class LoginFailedViewModel : BindableBase, INavigationAware
    {
        private readonly INavigator _navigator;
        private string _failedReason;

        public LoginFailedViewModel(INavigator navigator)
        {
            _navigator = navigator;
            TryAgainCommand = new DelegateCommand(TryAgain);
        }

        public string FailedReason
        {
            get => _failedReason;
            private set => SetValue(ref _failedReason, value);
        }

        public ICommand TryAgainCommand { get; }

        private void TryAgain()
        {
            _navigator.Navigate("Login");
        }

        public bool IsNavigationTarget()
        {
            return true;
        }

        public void OnNavigatedTo(IDictionary<string, object> navigationParameters)
        {
            FailedReason = navigationParameters["failedReason"].ToString();
        }
    }
}