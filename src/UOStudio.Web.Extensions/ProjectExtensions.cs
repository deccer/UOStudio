using System.Collections.Generic;
using System.Linq;
using UOStudio.Web.Contracts;
using UOStudio.Web.Data.Entities;

namespace UOStudio.Web.Extensions
{
    public static class ProjectExtensions
    {
        public static IEnumerable<ProjectDto> ToDto(this IEnumerable<Project> projects) => projects.Select(u => u.ToDto());

        public static IEnumerable<ProjectDto> ToDto(this IQueryable<Project> projects) => projects.Select(u => u.ToDto());

        public static ProjectDto ToDto(this Project project) => new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            ClientVersion = project.ClientVersion
        };
    }
}
