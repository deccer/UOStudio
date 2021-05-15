using System.Threading.Tasks;

namespace UOStudio.Client.Launcher.ViewModels
{
    public interface IAsyncLoadable
    {
        Task LoadAsync();
    }
}