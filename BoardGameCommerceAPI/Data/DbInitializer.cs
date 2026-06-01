using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;

public class DbInitializer
{
    private readonly IConfiguration _config;
    private readonly IPasswordHasher<Customer> _hasher;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IDbConnectionFactory _dbContext;

    public DbInitializer(
        IConfiguration config,
        IPasswordHasher<Customer> hasher,
        ILogger<DbInitializer> logger,
        IDbConnectionFactory dbContext
    )
    {
        _config = config;
        _hasher = hasher;
        _logger = logger;
        _dbContext = dbContext;
    }

    public void Initialize(bool reInitialize = false)
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        if (reInitialize)
        {
            DropTables(connection);

            CreateProductsTable(connection);
            CreateUsersTable(connection);
            CreateSalesTable(connection);
            CreateSalesProductsTable(connection);

            InsertProductData(connection);

            CreateAdmin(connection);
        }
    }

    private void CreateAdmin(SqliteConnection connection)
    {
        var admin = new Customer
        {
            Name = _config["AdminInfo:Name"],
            Email = _config["AdminInfo:Email"],
        };
        admin.PasswordHash = _hasher.HashPassword(admin, _config["AdminInfo:Password"]);

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                INSERT INTO users(id, name, email, password_hash)
                VALUES
                ( $Id,
                  $Name,
                  $Email,
                  $PasswordHash)
                ;
            """;

        command.Parameters.AddWithValue("$Id", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("$Name", admin.Name);
        command.Parameters.AddWithValue("$Email", admin.Email);
        command.Parameters.AddWithValue("$PasswordHash", admin.PasswordHash);

        try
        {
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new ApplicationException("Admin creation failed");
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }

    private static void DropTables(SqliteConnection connection)
    {
        var lst = new List<string> { "sales_products", "sales", "products", "users" };

        foreach (string table in lst)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {table};";

            command.ExecuteNonQuery();
        }
    }

    private static void CreateProductsTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = """
              CREATE TABLE IF NOT EXISTS products (
                id TEXT NOT NULL PRIMARY KEY ,
                name TEXT NOT NULL,
                yearpublished INTEGER NOT NULL,
                rank REAL NOT NULL,
                price REAL NOT NULL
            );
            """;

        command.ExecuteNonQuery();
    }

    private static void CreateSalesTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = """
              CREATE TABLE IF NOT EXISTS sales (
                id TEXT NOT NULL PRIMARY KEY ,
                customer_id INTEGER NOT NULL,
                date TEXT NOT NULL,
                time TEXT NOT NULL,
                FOREIGN KEY (customer_id)
                    REFERENCES users (id) ON DELETE CASCADE
            );
            """;

        command.ExecuteNonQuery();
    }

    private static void CreateUsersTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = """
              CREATE TABLE IF NOT EXISTS users (
                id TEXT NOT NULL PRIMARY KEY ,
                role TEXT NOT NULL DEFAULT 'customer',
                name TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL
            );
            """;

        command.ExecuteNonQuery();
    }

    private static void CreateSalesProductsTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = """
              CREATE TABLE IF NOT EXISTS sales_products (
                id TEXT NOT NULL PRIMARY KEY ,
                product_id TEXT NOT NULL,
                sale_id TEXT NOT NULL,
                quantity INTEGER NOT NULL,
                FOREIGN KEY (sale_id)
                    REFERENCES sales (id) ON DELETE CASCADE,
                FOREIGN KEY (product_id)
                    REFERENCES products (id)
            );
            """;

        command.ExecuteNonQuery();
    }

    public static void InsertProductData(SqliteConnection connection)
    {
        using (var transaction = connection.BeginTransaction())
        {
            var path = "BoardGameData/boardgames_data.csv";
            var insert_command = connection.CreateCommand();
            insert_command.CommandText = """
                INSERT INTO products(id, name, yearpublished, rank, price)
                VALUES
                ( $Id,
                  $Name,
                  $Year,
                  $Rank,
                  $Price)
                ;
                """;

            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;
                csvParser.CommentTokens = ["#"];

                // skip titles line
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    string[]? fields = csvParser.ReadFields();
                    if (fields is not null)
                    {
                        insert_command.Parameters.Clear();
                        insert_command.Parameters.AddWithValue("$Id", Guid.NewGuid().ToString());
                        insert_command.Parameters.AddWithValue("$Name", fields[0]);
                        insert_command.Parameters.AddWithValue("$Year", int.Parse(fields[1]));
                        insert_command.Parameters.AddWithValue("$Rank", float.Parse(fields[2]));
                        insert_command.Parameters.AddWithValue("$Price", float.Parse(fields[3]));

                        insert_command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}
