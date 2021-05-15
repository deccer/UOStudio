using System.Threading.Tasks;
using System.Windows.Input;

namespace UOStudio.Client.Launcher.Commands
{
    public interface IAsyncDelegateCommand : ICommand
    {
        Task ExecuteAsync();

        bool CanExecute();
    }
}
