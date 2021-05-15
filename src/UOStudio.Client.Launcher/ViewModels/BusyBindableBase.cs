using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace UOStudio.Client.Launcher.ViewModels
{
    public class BusyBindableBase : BindableBase
    {
        private bool _isBusy;
        private readonly ObservableCollection<Func<Task>> _pendingOperations;

        protected BusyBindableBase()
        {
            _pendingOperations = new ObservableCollection<Func<Task>>();
            _pendingOperations.CollectionChanged += (_, _) => { IsBusy = _pendingOperations.Count > 0; };
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetValue(ref _isBusy, value);
        }

        protected async Task AddBusyAsync(Func<Task> task)
        {
            _pendingOperations.Add(task);
            await task();
            _pendingOperations.Remove(task);
        }
    }
}