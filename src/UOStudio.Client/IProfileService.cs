namespace UOStudio.Client
{
    public interface IProfileService
    {
        Profile GetProfile(string profileName);

        string[] GetProfileNames();

        void DeleteProfile(Profile profile);

        void CreateProfile(
            string name,
            string serverName,
            int serverPort,
            string userName,
            string password);
    }
}
