using System.Threading.Tasks;
using System.Windows.Input;
using UOStudio.Client.Launcher.Commands;
using UOStudio.Client.Launcher.Messages;
using UOStudio.Client.Launcher.Services;

namespace UOStudio.Client.Launcher.ViewModels
{
    public sealed class ShellViewModel : BindableBase, IAsyncLoadable
    {
        private readonly INavigator _navigator;
        private readonly IUserContext _userContext;
        private object _currentViewModel;
        private string _userName;

        public ShellViewModel(
            INavigator navigator,
            IMessageBus messageBus,
            IUserContext userContext)
        {
            _navigator = navigator;
            _navigator.OnNavigated += OnNavigated;
            _userContext = userContext;
            messageBus.Subscribe<UserLoggedInMessage>(OnUserLoggedInMessage);

            LogoutCommand = new DelegateCommand(Logout);
        }

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set => SetValue(ref _currentViewModel, value);
        }

        public ICommand LogoutCommand { get; }

        public string UserName
        {
            get => _userName;
            set => SetValue(ref _userName, value);
        }

        public Task LoadAsync()
        {
            _navigator.Navigate("Login");
            return Task.CompletedTask;
        }

        private void Logout()
        {
            _userContext.UserCredentials = null;
            UserName = string.Empty;
            _navigator.Navigate("Login");
        }

        private Task OnUserLoggedInMessage(UserLoggedInMessage message)
        {
            UserName = message.UserName;
            return Task.CompletedTask;
        }

        private void OnNavigated(string uri, BindableBase navigatedToViewModel)
        {
            CurrentViewModel = navigatedToViewModel;
        }
    }
}