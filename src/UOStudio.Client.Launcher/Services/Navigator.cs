using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using Serilog;
using UOStudio.Client.Launcher.ViewModels;

namespace UOStudio.Client.Launcher.Services
{
    public sealed class Navigator : INavigator
    {
        private readonly ILogger _logger;
        private readonly Func<string, BindableBase> _navigationTargets;

        public Navigator(
            ILogger logger,
            Func<string, BindableBase> navigationTargets)
        {
            _logger = logger;
            _navigationTargets = navigationTargets;
        }

        public event Action<string, BindableBase> OnNavigated;

        public void Navigate(string uri, IDictionary<string, object> navigationParameters = null)
        {
            var navigationTarget = _navigationTargets(uri);
            if (navigationTarget is null)
            {
                _logger.Error("No navigation possible to {@uri}", uri);
                Application.Current.Shutdown();
            }
            if (navigationTarget is INavigationAware navigationAware)
            {
                if (!navigationAware.IsNavigationTarget())
                {
                    return;
                }

                navigationAware.OnNavigatedTo(navigationParameters ?? ImmutableDictionary<string, object>.Empty);
            }

            OnNavigated?.Invoke(uri, navigationTarget);
        }
    }
}