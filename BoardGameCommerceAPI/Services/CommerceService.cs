using Microsoft.Data.Sqlite;
public class CommerceService : ICommerceService
{
    private readonly IDbConnectionFactory _dbContext;

    public CommerceService(IDbConnectionFactory dbContext)
    {
        _dbContext = dbContext;

    }

    public List<Product>? GetAllProducts()
    {

        List<Product> rows = new ();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, yearpublished FROM products
        """;
        try
        {
        using var datareader = command.ExecuteReader();
        var i = 0;

        if (!datareader.HasRows) return rows;
        else
        {
            while (datareader.Read())
            {
                {
                    rows.Add(new Product{Name = datareader.GetString(1), YearPublished = int.Parse(datareader.GetString(2))});
                    rows[i].Id = datareader.GetGuid(0);
                    i++;
                }
            }
        }

        }
        catch (SqliteException ex)
        {
            var message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }
        return rows;
    }
        


    }