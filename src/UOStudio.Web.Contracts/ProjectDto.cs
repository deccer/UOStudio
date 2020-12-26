using System;

namespace UOStudio.Web.Contracts
{
    public class ProjectDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ClientVersion { get; set; }
    }
}
