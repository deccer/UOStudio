namespace UOStudio.Client
{
    public record Profile
    {
        public string Name { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
