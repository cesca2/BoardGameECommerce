using Microsoft.Data.Sqlite;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    // IConfiguration available in DI container
    public SqliteConnectionFactory(IConfiguration config, string dataSource)
    {
        _connectionString = config.GetConnectionString(dataSource);
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}