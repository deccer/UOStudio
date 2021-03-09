using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UOStudio.Server.Data
{
    public record Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ProjectTemplate Template { get; set; }

        public bool IsPublic { get; set; }

        public User CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public virtual ICollection<User> AllowedUsers { get; set; }
    }
}
