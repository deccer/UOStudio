using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using UOStudio.Client.Launcher.Commands;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Data.Repositories;
using UOStudio.Client.Launcher.Services;

namespace UOStudio.Client.Launcher.ViewModels
{
    public class ProfilesViewModel : BindableBase, IAsyncLoadable
    {
        private ObservableCollection<LookupItem> _profiles;
        private LookupItem _selectedProfileLookupItem;
        private ProfileDto _selectedProfile;
        
        private readonly ILogger _logger;
        private readonly INavigator _navigator;
        private readonly IProfileRepository _profileRepository;

        public IAsyncDelegateCommand DeleteProfileCommand { get; }
        public IAsyncDelegateCommand AddProfileCommand { get; }
        public IAsyncDelegateCommand UpdateProfileCommand { get; }

        public ObservableCollection<LookupItem> Profiles
        {
            get => _profiles;
            set => SetValue(ref _profiles, value);
        }

        public LookupItem SelectedProfileLookupItem
        {
            get => _selectedProfileLookupItem;
            set => SetValue(ref _selectedProfileLookupItem, value, OnSelectedProfileChanged);
        }

        public ProfileDto SelectedProfile
        {
            get => _selectedProfile;
            set => SetValue(ref _selectedProfile, value);
        }

        public ProfilesViewModel(
            ILogger logger,
            INavigator navigator,
            IProfileRepository profileRepository)
        {
            _logger = logger;
            _navigator = navigator;
            _profileRepository = profileRepository;

            DeleteProfileCommand = new AsyncDelegateCommand(async () => await DeleteProfileAsync());
            AddProfileCommand = new AsyncDelegateCommand(async () => await AddProfileAsync());
            UpdateProfileCommand = new AsyncDelegateCommand(async () => await UpdateProfileAsync());
        }

        public Task LoadAsync()
        {
            var profiles = _profileRepository.GetProfiles();
            Profiles = new ObservableCollection<LookupItem>(profiles);

            return Task.CompletedTask;
        }

        private async Task DeleteProfileAsync()
        {
            await _profileRepository.DeleteProfileAsync(_selectedProfileLookupItem.Id);
            await LoadAsync();
        }

        private async Task AddProfileAsync()
        {
            await _profileRepository.AddProfileAsync(_selectedProfile);
            await LoadAsync();
        }

        private async Task UpdateProfileAsync()
        {
            await _profileRepository.UpdateProfileAsync(SelectedProfileLookupItem.Id, SelectedProfile);
        }

        private async void OnSelectedProfileChanged()
        {
            if (SelectedProfileLookupItem == null)
            {
                return;
            }

            var profile = await _profileRepository.GetProfileAsync(SelectedProfileLookupItem.Id);
            SelectedProfile = profile;
        }
    }
}
