using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace UOStudio.Client.UI
{
    public class WindowProvider : IWindowProvider
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<Window> _windows;

        public WindowProvider(
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _windows = new List<Window>();
        }

        public void Draw()
        {
            foreach (var window in _windows)
            {
                window.Draw();
            }
        }

        public TWindow GetWindow<TWindow>()
            where TWindow : Window
            => _windows.OfType<TWindow>().FirstOrDefault();

        public void Load()
        {
            var windows = _serviceProvider.GetServices<Window>();
            foreach (var window in windows)
            {
                _windows.Add(window);
            }
        }
    }
}
