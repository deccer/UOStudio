using System.Security.Principal;

namespace UOStudio.Client.Services
{
    public interface ITokenHandler
    {
        IPrincipal GetUserFromAccessToken(string accessToken);
    }
}
