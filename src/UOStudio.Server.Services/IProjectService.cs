using System;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface IProjectService
    {
        Result<bool> CreateProject(string projectTemplatePath, int projectId);

        string GetProjectPath(int projectId);

        Result<Guid> PrepareProjectForClientDownload(int projectId);
    }
}
