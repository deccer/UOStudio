using System.Collections.Generic;
using JetBrains.Annotations;

namespace UOStudio.Server.Data
{
    public sealed record Permission
    {
        public static readonly Permission CanConnect = new(1, nameof(CanConnect));
        public static readonly Permission CanCreateProject = new(2, nameof(CanCreateProject));
        public static readonly Permission CanDeleteProject = new(3, nameof(CanDeleteProject));
        public static readonly Permission CanEditProject = new(4, nameof(CanEditProject));
        public static readonly Permission CanJoinProject = new(5, nameof(CanJoinProject));

        public static readonly Permission CanCreateProjectTemplate = new(6, nameof(CanCreateProjectTemplate));
        public static readonly Permission CanDeleteProjectTemplate = new(7, nameof(CanDeleteProjectTemplate));
        public static readonly Permission CanModifyProjectTemplate = new(8, nameof(CanModifyProjectTemplate));

        public static readonly Permission CanCreateUser = new(9, nameof(CanCreateUser));
        public static readonly Permission CanDeleteUser = new(10, nameof(CanDeleteUser));
        public static readonly Permission CanModifyUser = new(11, nameof(CanModifyUser));
        public static readonly Permission CanKickUser = new(12, nameof(CanKickUser));
        public static readonly Permission CanBanUser = new(13, nameof(CanBanUser));
        public static readonly Permission CanListUser = new(14, nameof(CanListUser));

        public static readonly Permission CanAddUserToProject = new(15, nameof(CanAddUserToProject));
        public static readonly Permission CanRemoveUserFromProject = new(16, nameof(CanRemoveUserFromProject));

        public static readonly Permission[] AllPermissions =
        {
            CanConnect,
            CanCreateProject,
            CanDeleteProject,
            CanEditProject,
            CanJoinProject,
            CanCreateProjectTemplate,
            CanDeleteProjectTemplate,
            CanModifyProjectTemplate,
            CanCreateUser,
            CanDeleteUser,
            CanModifyUser,
            CanKickUser,
            CanBanUser,
            CanAddUserToProject,
            CanRemoveUserFromProject,
            CanListUser,
        };

        [UsedImplicitly]
        public Permission()
        {
        }

        private Permission(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
