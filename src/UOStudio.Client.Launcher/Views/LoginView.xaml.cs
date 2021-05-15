using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UOStudio.Client.Launcher.ViewModels;

namespace UOStudio.Client.Launcher.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IAsyncLoadable loadable)
            {
                await loadable.LoadAsync();
            }
        }
    }
}