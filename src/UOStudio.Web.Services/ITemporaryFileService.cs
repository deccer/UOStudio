using System;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Web.Services
{
    public interface ITemporaryFileService
    {
        Task<Guid> CreateFile(Guid projectId);

        Task<Result<Stream>> GetFileAsync(Guid projectId);
    }
}
