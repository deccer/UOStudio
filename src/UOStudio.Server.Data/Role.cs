using System.Linq;

namespace UOStudio.Server.Data
{
    public static class Role
    {
        public static readonly Permission[] Administrator = Permission.AllPermissions;

        public static readonly Permission[] Editor = new[]
        {
            Permission.CanConnect,
            Permission.CanEditProject
        };

        public static readonly Permission[] BlockedUser = System.Array.Empty<Permission>();

        public static readonly Permission[] ProjectAdmin = new[]
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
