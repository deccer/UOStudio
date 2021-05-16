using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface IProjectTemplateService
    {
        Task<Result<string>> CreateProjectTemplateAsync(string templateName);
    }
}
