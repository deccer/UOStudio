using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Shared.Network;
using UOStudio.Web.Contracts;

namespace UOStudio.Web.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();

        Task<Result<ProjectDto>> GetProjectAsync(Guid projectId, Guid userId);

        Task<Result<Guid>> CreateProjectAsync(string projectName, string projectDescription, string projectClientVersion, Guid userId);

        Task<Result> DeleteProjectAsync(Guid projectId, Guid userId);

        Task<Result<JoinResult>> JoinProjectAsync(Guid projectId, Guid userId, byte[] projectClientHash);
    }
}
