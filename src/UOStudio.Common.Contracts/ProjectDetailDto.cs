using System;

namespace UOStudio.Common.Contracts
{
    public record ProjectDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string Template { get; set; }

        public string ClientVersion { get; set; }
    }
}
