using System.Windows;
using UOStudio.Client.Launcher.ViewModels;

namespace UOStudio.Client.Launcher.Views
{
    public partial class ShellView
    {
        private readonly ShellViewModel _shellViewModel;
        
        public ShellView(
            ShellViewModel shellViewModel)
        {
            InitializeComponent();
            _shellViewModel = shellViewModel;
            
            DataContext = _shellViewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _shellViewModel.LoadAsync();
        }
    }
}