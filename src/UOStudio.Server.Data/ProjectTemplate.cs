﻿using System;
using JetBrains.Annotations;

namespace UOStudio.Server.Data
{
    public record ProjectTemplate
    {
        public ProjectTemplate(string name, string clientVersion, string location)
        {
            Name = name;
            ClientVersion = clientVersion;
            Location = location;
        }

        [UsedImplicitly]
        public ProjectTemplate()
        {
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string ClientVersion { get; set; }

        public string Location { get; set; }

        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
