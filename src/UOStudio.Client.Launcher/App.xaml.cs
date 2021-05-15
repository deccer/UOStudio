using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Launcher.Data;
using UOStudio.Client.Launcher.Services;
using UOStudio.Client.Launcher.ViewModels;
using UOStudio.Client.Launcher.Views;

namespace UOStudio.Client.Launcher
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceProvider = BuildServiceProvider();
            using (var scope = serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProfileDbContext>>();
                using var context = contextFactory.CreateDbContext();
                context.Database.Migrate();
            }

            MainWindow = serviceProvider.GetService<ShellView>();
            MainWindow?.Show();
        }

        private static IServiceProvider BuildServiceProvider()
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

            services.AddDbContextFactory<ProfileDbContext>((provider, options) =>
            {
                var cs = provider.GetRequiredService<ClientSettings>();
                var profilesDb = string.IsNullOrEmpty(Path.GetDirectoryName(cs.ProfilesDirectory))
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles", cs.ProfilesDirectory)
                    : cs.ProfilesDirectory;
                var profilesDbDirectory = Path.GetDirectoryName(profilesDb);
                Directory.CreateDirectory(profilesDbDirectory);

                options.UseSqlite($"Data Source={profilesDb}");
            });
            services.AddSingleton<IProfileService, ProfileService>();

            services.AddTransient<ITokenClient, TokenClient>();
            services.AddTransient<UoStudioClientTokenHandler>();
            services.AddHttpClient<IUoStudioClient, UoStudioClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            }).AddHttpMessageHandler<UoStudioClientTokenHandler>();

            return services.BuildServiceProvider();
        }
    }
}
