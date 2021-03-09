using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.Mappers
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name
            };
        }

        public static ProjectDetailDto ToDetailDto(this Project project)
        {
            return new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                CreatedBy = project.CreatedBy.Name,
                Template = project.Template.Name,
                ClientVersion = project.Template.ClientVersion
            };
        }
    }
}
