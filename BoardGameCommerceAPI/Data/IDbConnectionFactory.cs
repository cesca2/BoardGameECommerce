
using Microsoft.Data.Sqlite;
public interface IDbConnectionFactory
{
    SqliteConnection CreateConnection();
}