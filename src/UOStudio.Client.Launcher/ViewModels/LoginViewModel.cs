using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Serilog;
using UOStudio.Client.Launcher.Commands;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Data.Repositories;
using UOStudio.Client.Launcher.Messages;
using UOStudio.Client.Launcher.Services;

namespace UOStudio.Client.Launcher.ViewModels
{
    public class LoginViewModel : BusyBindableBase, IAsyncLoadable
    {
        private readonly ILogger _logger;
        private readonly INavigator _navigator;
        private readonly IUserContext _userContext;
        private readonly IUOStudioClient _iuoStudioClient;
        private readonly IMessageBus _messageBus;
        private readonly IProfileRepository _profileRepository;

        private LookupItem _selectedProfile;

        private string _userName;
        private string _password;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<LookupItem> _profileNames;

        public LoginViewModel(
            ILogger logger,
            INavigator navigator,
            IUserContext userContext,
            IUOStudioClient iuoStudioClient,
            IMessageBus messageBus,
            IProfileRepository profileRepository)
        {
            _logger = logger;
            _navigator = navigator;
            _userContext = userContext;
            _iuoStudioClient = iuoStudioClient;
            _messageBus = messageBus;
            _profileRepository = profileRepository;

            LoginCommand = new AsyncDelegateCommand(Login);
            CancelLoginCommand = new AsyncDelegateCommand(CancelLogin);
            EditProfilesCommand = new DelegateCommand(EditProfiles);
        }

        public string UserName
        {
            get => _userName;
            set => SetValue(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public ObservableCollection<LookupItem> ProfileNames
        {
            get => _profileNames;
            set => SetValue(ref _profileNames, value);
        }

        public LookupItem SelectedProfile
        {
            get => _selectedProfile;
            set => SetValue(ref _selectedProfile, value);
        }

        public IAsyncDelegateCommand LoginCommand { get; }

        public IAsyncDelegateCommand CancelLoginCommand { get; }

        public ICommand EditProfilesCommand {get;}

        public Task LoadAsync()
        {
            ProfileNames = new ObservableCollection<LookupItem>(_profileRepository.GetProfiles());
            return Task.CompletedTask;
        }

        private Task CancelLogin()
        {
            _cancellationTokenSource?.Cancel();
            IsBusy = false;
            return Task.CompletedTask;
        }

        private async Task Login()
        {
            await AddBusyAsync(async () =>
            {
                try
                {
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

                    var profile = await _profileRepository.GetProfileAsync(_selectedProfile.Id);
                    if (profile == null)
                    {
                        _logger.Error("Unable to fetch profile");
                    }
                    else
                    {
                        _userContext.ApiBaseUri = new Uri(profile.ApiBaseUri);
                        _userContext.AuthBaseUri = new Uri(profile.AuthBaseUri);
                        _userContext.UserCredentials = new UserCredentials { UserName = profile.UserName, Password = profile.Password };

                        var getProjectsResult = await _iuoStudioClient.GetProjectsAsync(CancellationToken.None);
                        if (getProjectsResult.IsFailure)
                        {
                            var failedReason = getProjectsResult.Error;
                            var navigationParameters = new Dictionary<string, object>
                        {
                            { nameof(failedReason), failedReason }
                        };
                            _navigator.Navigate("LoginFailed", navigationParameters);
                        }
                        else
                        {
                            await _messageBus.PublishWaitAsync(new UserLoggedInMessage(UserName));
                            var navigationParameters = new Dictionary<string, object>
                            {
                                { "projects", getProjectsResult.Value }
                            };
                            _navigator.Navigate("Main", navigationParameters);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "{@Message}", exception.Message);

                    var failedReason = exception.Message;
                    var navigationParameters = new Dictionary<string, object>
                    {
                        { nameof(failedReason), failedReason }
                    };
                    _navigator.Navigate("LoginFailed", navigationParameters);
                    
                    _cancellationTokenSource?.Dispose();
                    IsBusy = false;
                }
            });
        }

        private void EditProfiles()
        {
            _navigator.Navigate("Profiles");
        }
    }
}