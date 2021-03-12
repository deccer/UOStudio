using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface IProjectService
    {
        Result<bool> CreateProject(string projectTemplatePath, string name);
    }
}
