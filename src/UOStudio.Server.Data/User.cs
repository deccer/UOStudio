using System.Collections.Generic;
using JetBrains.Annotations;

namespace UOStudio.Server.Data
{
    public sealed record User
    {
        public User(
            int id,
            string name,
            byte[] password,
            byte[] nonce,
            params Permission[] permissions)
        {
            Id = id;
            Name = name;
            Password = password;
            Nonce = nonce;
            Permissions = permissions;
        }

        [UsedImplicitly]
        public User()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Password { get; set; }

        public byte[] Nonce { get; set; }

        public string RefreshToken { get; set; }

        public string ConnectionTicket { get; set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        public ICollection<Project> Projects { get; set; } = new List<Project>();

        public bool HasPermission(Permission permission)
        {
            return Permissions.Contains(permission);
        }
    }
}
