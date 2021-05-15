using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Client.Services
{
    public interface ITokenService
    {
        Task<Result<string>> RetrieveTokenAsync(string userName, string password);
    }
}
