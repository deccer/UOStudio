namespace UOStudio.Client.Launcher.Data
{
    public class Profile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AuthBaseUri { get; set; }

        public string ApiBaseUri { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}