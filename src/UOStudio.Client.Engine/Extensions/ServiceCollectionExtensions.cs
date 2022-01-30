using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UOStudio.Client.Engine.Graphics;

namespace UOStudio.Client.Engine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEngineKit(this IServiceCollection services, IConfiguration configuration)
        {
            var windowSettings = WindowSettings.Default;
            configuration.Bind(nameof(WindowSettings), windowSettings);

            var contextSettings = ContextSettings.Default;
            configuration.Bind(nameof(ContextSettings), contextSettings);

            services.AddSingleton(windowSettings);
            services.AddSingleton(contextSettings);

            services.AddSingleton<IInputLayoutMapper, InputLayoutMapper>();
            services.AddSingleton<IInputLayoutProvider, InputLayoutProvider>();
            services.AddSingleton<IWindowFactory, WindowFactory>();
            services.AddSingleton<IGraphicsDevice, GraphicsDevice>();

            services.AddSingleton<ITextureLibrary, TextureLibrary>();
            services.AddSingleton<IMaterialLibrary, MaterialLibrary>();
            services.AddSingleton<IMeshLoader, MeshLoader>();
            services.AddSingleton<IMeshLibrary, MeshLibrary>();
            services.AddSingleton<IMaterialLibrary, MaterialLibrary>();
            services.AddSingleton<ITextureLibrary, TextureLibrary>();
            services.AddSingleton<IShaderProgramLibrary, ShaderProgramLibrary>();
        }
    }
}
