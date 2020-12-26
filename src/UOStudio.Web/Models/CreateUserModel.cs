using UOStudio.Core;

namespace UOStudio.Web.Models
{
    public class CreateUserModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public Permissions Permissions { get; set; }
    }
}
