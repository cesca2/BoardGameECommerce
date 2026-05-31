using Microsoft.Data.Sqlite;

public class ProductService : IProductService
{
    private readonly IDbConnectionFactory _dbContext;

    public ProductService(IDbConnectionFactory dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Product>? GetAllProducts()
    {
        List<Product> rows = new();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT id, name, yearpublished, price FROM products
            """;
        try
        {
            using var datareader = command.ExecuteReader();
            var i = 0;

            if (!datareader.HasRows)
                return rows;
            else
            {
                while (datareader.Read())
                {
                    {
                        rows.Add(
                            new Product
                            {
                                Name = datareader.GetString(1),
                                YearPublished = int.Parse(datareader.GetString(2)),
                                Price = float.Parse(datareader.GetString(3)),
                            }
                        );
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

    public Product? GetProductById(Guid id)
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT id, name, yearpublished, price FROM products WHERE id = @id;
            """;
        var sql_id = id.ToString();
        command.Parameters.AddWithValue("@id", sql_id);

        try
        {
            using var datareader = command.ExecuteReader();

            while (datareader.Read())
            {
                var product = new Product
                {
                    Name = datareader.GetString(1),
                    YearPublished = int.Parse(datareader.GetString(2)),
                    Price = float.Parse(datareader.GetString(3)),
                };
                product.Id = datareader.GetGuid(0);

                return product;
            }
            return null;
        }
        catch (SqliteException ex)
        {
            var message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }
    }
}
