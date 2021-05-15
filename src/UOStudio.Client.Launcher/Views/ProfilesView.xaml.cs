using System.Windows.Controls;
using UOStudio.Client.Launcher.ViewModels;

namespace UOStudio.Client.Launcher.Views
{
    public partial class ProfilesView : UserControl
    {
        public ProfilesView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is IAsyncLoadable loadable)
            {
                await loadable.LoadAsync();
            }
        }
    }
}