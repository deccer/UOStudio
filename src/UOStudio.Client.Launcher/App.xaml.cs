using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Data;
using UOStudio.Client.Launcher.Services;
using UOStudio.Client.Launcher.ViewModels;
using UOStudio.Client.Launcher.Views;

namespace UOStudio.Client.Launcher
{
    public partial class App
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _serviceProvider = BuildServiceProvider();

            MainWindow = _serviceProvider.GetService<ShellView>();
            MainWindow?.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var clientSettings = new ClientSettings();
            configuration.Bind("ClientSettings", clientSettings);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddSingleton(clientSettings);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IDialogCoordinator, DialogCoordinator>();
            services.AddTransient<Func<string, BindableBase>>(provider => name =>
            {
                var viewModelsNamespace = typeof(BindableBase).Namespace;
                var typeName = $"{viewModelsNamespace}.{name}ViewModel";
                var type = typeof(BindableBase).Assembly.GetType(typeName);
                if (type == null)
                {
                    var logger = provider.GetRequiredService<ILogger>();
                    logger.Fatal("Unable to resolve type {@TypeName}", typeName);
                    Current.Shutdown();
                }

                var instance = provider.GetService(type!);
                return instance as BindableBase;
            });
            services.AddSingleton<INavigator, Navigator>();
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<LoginFailedViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ShellView>();
            services.AddSingleton<ProfilesViewModel>();
            services.AddSingleton<IUserContext, UserContext>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IClientStarterService, ClientStarterService>();

            services.AddProfileDatabase();
            services.AddTransient<ITokenClient, TokenClient>();
            services.AddTransient<UOStudioClientTokenHandler>();
            services.AddHttpClient<IUOStudioClient, IuoStudioClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            }).AddHttpMessageHandler<UOStudioClientTokenHandler>();

            return services.BuildServiceProvider();
        }
    }
}
