using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Client.UI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWindows(this ServiceCollection services)
        {
            services.AddSingleton<IWindowProvider, WindowProvider>();

            var windowTypes = typeof(Window).Assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(typeof(Window)) && type.IsClass && !type.IsAbstract);
            foreach (var windowType in windowTypes)
            {
                services.AddSingleton(typeof(Window), windowType);
            }
        }
    }
}
