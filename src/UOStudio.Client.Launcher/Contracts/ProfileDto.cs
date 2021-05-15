using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOStudio.Client.Launcher.Contracts
{
    public class ProfileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string AuthBaseUri { get; set; }

        public string ApiBaseUri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
