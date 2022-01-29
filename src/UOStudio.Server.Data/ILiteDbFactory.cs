using LiteDB.Async;

namespace UOStudio.Server.Data
{
    public interface ILiteDbFactory
    {
        ILiteDatabaseAsync CreateLiteDatabase();
    }
}
