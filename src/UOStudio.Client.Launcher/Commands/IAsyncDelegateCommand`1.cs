using System.Threading.Tasks;
using System.Windows.Input;

namespace UOStudio.Client.Launcher.Commands
{
    public interface IAsyncDelegateCommand<in T> : ICommand
    {
        Task ExecuteAsync(T parameter);

        bool CanExecute(T parameter);
    }
}
