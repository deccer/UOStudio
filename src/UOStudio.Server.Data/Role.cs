using System.Collections.Generic;
using System.Linq;

namespace UOStudio.Server.Data
{
    public static class Role
    {
        public static readonly IReadOnlyCollection<Permission> Administrator = Permission.AllPermissions;

        public static readonly IReadOnlyCollection<Permission> Editor = new[]
        {
            Permission.CanConnect,
            Permission.CanEditProject
        };

        public static readonly IReadOnlyCollection<Permission> BlockedUser = System.Array.Empty<Permission>();

        public static readonly IReadOnlyCollection<Permission> ProjectAdmin = new[]
        {
            Permission.CanConnect,
            Permission.CanEditProject,
            Permission.CanAddUserToProject,
            Permission.CanRemoveUserFromProject,
        };

        public static bool IsAdministrator(User user)
        {
            return Administrator.All(permission => user.Permissions.Contains(permission));
        }
    }
}
