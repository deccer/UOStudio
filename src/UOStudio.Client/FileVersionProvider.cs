using System.Reflection;
using UOStudio.Client.Engine.Windows;

namespace UOStudio.Client
{
    public sealed class FileVersionProvider : IFileVersionProvider
    {
        public string GetVersion()
        {
            var assemblyFileVersionAttribute = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<AssemblyFileVersionAttribute>();
            return assemblyFileVersionAttribute?.Version;
        }
    }
}
