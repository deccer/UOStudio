using System;

namespace UOStudio.Server.Data
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public string ClientVersion { get; set; }

        public byte[] Version { get; set; }

        public Account CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Account ModifiedBy { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}
