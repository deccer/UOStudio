using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using UOStudio.Client.Launcher.Commands;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Services;

namespace UOStudio.Client.Launcher.ViewModels
{
    public class MainViewModel : BindableBase, INavigationAware
    {
        private readonly INavigator _navigator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IClientStarterService _clientStarterService;
        private readonly IUserContext _userContext;
        private ObservableCollection<ProjectDto> _projects;

        public ObservableCollection<ProjectDto> Projects
        {
            get => _projects;
            set => SetValue(ref _projects, value);
        }

        public IAsyncDelegateCommand CreateProjectCommand { get; }

        public IAsyncDelegateCommand EditProjectTemplatesCommand { get; }

        public IAsyncDelegateCommand<ProjectDto> JoinProjectCommand { get; }

        public IAsyncDelegateCommand<ProjectDto> RemoveProjectCommand { get; }

        public MainViewModel(
            INavigator navigator,
            IDialogCoordinator dialogCoordinator,
            IClientStarterService clientStarterService)
        {
            _navigator = navigator;
            _dialogCoordinator = dialogCoordinator;
            _clientStarterService = clientStarterService;

            CreateProjectCommand = new AsyncDelegateCommand(async () => await CreateProjectAsync());
            EditProjectTemplatesCommand = new AsyncDelegateCommand(async () => await EditProjectTemplatesAsync());
            JoinProjectCommand = new AsyncDelegateCommand<ProjectDto>(async project => await JoinProjectAsync(project));
            RemoveProjectCommand = new AsyncDelegateCommand<ProjectDto>(async project => await RemoveProjectAsync(project));
        }

        public bool IsNavigationTarget()
        {
            return true;
        }

        public void OnNavigatedTo(IDictionary<string, object> navigationParameters)
        {
            var projects = navigationParameters["projects"] as IImmutableList<ProjectDto>;

            Projects = new ObservableCollection<ProjectDto>(projects ?? Enumerable.Empty<ProjectDto>());
        }

        private async Task CreateProjectAsync()
        {
            try
            {
                var result = await _dialogCoordinator.ShowLoginAsync(this, "Title", "Message", new LoginDialogSettings { AnimateShow = true });

                var r2 = await _dialogCoordinator.ShowInputAsync(this, "Title", "Message", new MetroDialogSettings { AnimateHide = true, AnimateShow = true, ColorScheme = MetroDialogColorScheme.Accented });

                var r3 = _dialogCoordinator.ShowModalMessageExternal(this, "Title", "Message");
            }
            catch (Exception ex)
            {
                var x = ex.ToString();
            }
        }

        private Task EditProjectTemplatesAsync()
        {
            return Task.CompletedTask;
        }

        private async Task JoinProjectAsync(ProjectDto project)
        {
            var clientStartedResult = _clientStarterService.StartClientAsync(project);
            if (clientStartedResult.IsSuccess)
            {
                _dialogCoordinator.ShowModalMessageExternal(this, "Join Project", project.Name);
            }

            await Task.Delay(10);
        }

        private async Task RemoveProjectAsync(ProjectDto project)
        {
            await Task.Delay(10);
            _dialogCoordinator.ShowModalMessageExternal(this, "Remove Project", project.Name);
        }
    }
}
