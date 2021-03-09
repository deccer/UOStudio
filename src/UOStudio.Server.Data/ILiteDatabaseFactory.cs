using System.Threading.Tasks;
using LiteDB.Async;

namespace UOStudio.Server.Data
{
    public interface ILiteDatabaseFactory
    {
        Task<ILiteDatabaseAsync> OpenDatabaseAsync();
    }
}
