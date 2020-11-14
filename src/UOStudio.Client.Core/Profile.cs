using UOStudio.Core;

namespace UOStudio.Client.Core
{
    public class Profile : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string AccountName { get; set; }

        public string AccountPassword { get; set; }

        public override void CopyFrom(Entity entity)
        {
            if (entity is Profile otherProfile)
            {
                Name = otherProfile.Name;
                Description = otherProfile.Description;
                ServerName = otherProfile.ServerName;
                ServerPort = otherProfile.ServerPort;
                AccountName = otherProfile.AccountName;
                AccountPassword = otherProfile.AccountPassword;
            }
        }
    }
}
