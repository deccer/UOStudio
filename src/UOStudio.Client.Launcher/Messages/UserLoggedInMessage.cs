namespace UOStudio.Client.Launcher.Messages
{
    public readonly struct UserLoggedInMessage
    {
        public string UserName { get; }

        public UserLoggedInMessage(string userName)
        {
            UserName = userName;
        }
    }
}