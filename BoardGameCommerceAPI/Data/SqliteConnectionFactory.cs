using Microsoft.Data.Sqlite;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    // IConfiguration available in DI container
    public SqliteConnectionFactory(IConfiguration config, string dataSource)
    {
        var dataSourceString = config.GetConnectionString(dataSource) ?? "";
        if (dataSource == "Testing")
        {
            dataSourceString += Guid.NewGuid().ToString() + ".db";
        }
        Console.WriteLine(dataSourceString);

        var dataSourcePassword = config["Database:Password"];
        _connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = dataSourceString,
            Password = dataSourcePassword,
        }.ToString();
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
