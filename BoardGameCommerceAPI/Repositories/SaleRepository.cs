using Microsoft.Data.Sqlite;

public class SaleRepository : ISaleRepository
{
    private readonly IDbConnectionFactory _dbContext;
    private readonly ILogger<SaleRepository> _logger;

    public SaleRepository(IDbConnectionFactory dbContext, ILogger<SaleRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public int? CountSales()
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT COUNT(*)
                FROM sales
            """;
        try
        {
            var datacount = command.ExecuteScalar();
            return Convert.ToInt32(datacount);
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }

    public List<SalesProduct>? GetSalesProductsCount(int limit = -1)
    {
        List<SalesProduct> rows = new();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT products.id, products.name, COUNT(*) AS total_items
            FROM products JOIN sales_products
            ON products.id = sales_products.product_id
            GROUP BY  products.id, products.name
            ORDER BY total_items DESC
            LIMIT $Limit;
            """;
        command.Parameters.Add(new SqliteParameter("$Limit", limit));

        try
        {
            using var datareader = command.ExecuteReader();

            if (!datareader.HasRows)
                return rows;
            else
            {
                while (datareader.Read())
                {
                    Guid Id = datareader.GetGuid(0);
                    var sale = rows.FirstOrDefault(i => i.Id == Id);
                    if (sale == null)
                    {
                        sale = new SalesProduct
                        {
                            Id = datareader.GetGuid(0),
                            Name = datareader.GetString(1),
                            SalesItemsTotal = datareader.GetInt32(2),
                        };
                        rows.Add(sale);
                    }
                }
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
        return rows;
    }

    public Sale? GetSale(Guid id)
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT sales.id, customer_id, product_id, quantity
            FROM sales JOIN sales_products ON sales.id = sales_products.sale_id
            WHERE sales.id = $id;
            """;
        command.Parameters.Add(new SqliteParameter("$id", id.ToString()));

        try
        {
            using var datareader = command.ExecuteReader();

            if (!datareader.HasRows)
                return null;
            else
            {
                Sale? sale = null;
                while (datareader.Read())
                {
                    Guid Id = datareader.GetGuid(0);

                    if (sale == null)
                    {
                        sale = new Sale
                        {
                            Customer_Id = datareader.GetGuid(1),
                            QuantitiesByProductID = new(),
                            Id = datareader.GetGuid(0),
                        };
                    }
                    sale.QuantitiesByProductID[datareader.GetGuid(2)] = datareader.GetInt32(3);
                }
                return sale;
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }

    public List<Sale>? GetAllSales()
    {
        List<Sale> rows = new();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT sales.id, customer_id, product_id, quantity, date, time
            FROM sales JOIN sales_products ON sales.id = sales_products.sale_id ;
            """;
        try
        {
            using var datareader = command.ExecuteReader();

            if (!datareader.HasRows)
                return rows;
            else
            {
                while (datareader.Read())
                {
                    Guid Id = datareader.GetGuid(0);
                    var sale = rows.FirstOrDefault(i => i.Id == Id);
                    if (sale == null)
                    {
                        sale = new Sale
                        {
                            Customer_Id = datareader.GetGuid(1),
                            QuantitiesByProductID = new(),
                            Id = datareader.GetGuid(0),
                            Date = DateOnly.Parse(datareader.GetString(4)),
                            Time = TimeOnly.Parse(datareader.GetString(5)),
                        };
                        rows.Add(sale);
                    }
                    sale.QuantitiesByProductID[datareader.GetGuid(2)] = datareader.GetInt32(3);
                }
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
        return rows;
    }

    public int CreateSale(Sale newSale)
    {
        try
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                using var sales_command = connection.CreateCommand();
                sales_command.CommandText = """
                        INSERT INTO sales(id, customer_id, date, time)
                        VALUES
                        ( $Id,
                          $Customer_Id,
                          $Date,
                          $Time )
                        ;
                    """;

                sales_command.Parameters.AddWithValue("$Id", newSale.Id.ToString());
                sales_command.Parameters.AddWithValue(
                    "$Customer_Id",
                    newSale.Customer_Id.ToString()
                );
                sales_command.Parameters.AddWithValue("$Time", newSale.Time.ToString());
                sales_command.Parameters.AddWithValue("$Date", newSale.Date.ToString());

                int sales_rowsAffected = sales_command.ExecuteNonQuery();

                using var sales_products_command = connection.CreateCommand();
                sales_products_command.CommandText = """
                        INSERT INTO sales_products(id, sale_id, product_id, quantity)
                        VALUES
                        ( $Id,
                          $Sale_Id,
                          $Product_Id,
                          $Quantity
                        )
                        ;
                    """;

                int total_rows_affected = 0;

                foreach (var item in newSale.QuantitiesByProductID)
                {
                    sales_products_command.Parameters.Clear();
                    sales_products_command.Parameters.AddWithValue(
                        "$Id",
                        Guid.NewGuid().ToString()
                    );
                    sales_products_command.Parameters.AddWithValue(
                        "$Sale_Id",
                        newSale.Id.ToString()
                    );
                    sales_products_command.Parameters.AddWithValue(
                        "$Product_Id",
                        item.Key.ToString()
                    );
                    sales_products_command.Parameters.AddWithValue("$Quantity", item.Value);

                    total_rows_affected += sales_products_command.ExecuteNonQuery();
                }

                transaction.Commit();

                return sales_rowsAffected;
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }

    public int DeleteSale(Guid id)
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                    DELETE FROM sales
                    WHERE id = $ID
                    ;
            """;
        command.Parameters.AddWithValue("$ID", id.ToString());

        try
        {
            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected;
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }
}
