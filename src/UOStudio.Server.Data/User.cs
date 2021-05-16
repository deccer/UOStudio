using System.Collections.Generic;
using JetBrains.Annotations;

namespace UOStudio.Server.Data
{
    public record User
    {
        public User(int id, string name, string password, params Permission[] permissions)
        {
            Id = id;
            Name = name;
            Password = password;
        }

        [UsedImplicitly]
        public User()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }

        public string ConnectionTicket { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public bool HasPermission(Permission permission)
        {
            return Permissions.Contains(permission);
        }
    }
}
