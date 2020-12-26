using System;
using UOStudio.Core;

namespace UOStudio.Client.Core
{
    public sealed class Account
    {
        public Account(Guid id, string name, Permissions permissions)
        {
            Id = id;
            Name = name;
            Permissions = permissions;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Permissions Permissions { get; set; }
    }
}
