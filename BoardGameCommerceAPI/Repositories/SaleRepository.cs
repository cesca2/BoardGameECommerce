using System.Globalization;
using CommerceAPI.Controllers;
using Microsoft.Data.Sqlite;

public class SaleRepository : ISaleRepository
{
    private readonly IDbConnectionFactory _dbContext;

    public SaleRepository(IDbConnectionFactory dbContext)
    {
        _dbContext = dbContext;
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
                            Customer_Id = datareader.GetString(1),
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
            var message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
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
                            Customer_Id = datareader.GetString(1),
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
            var message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }
        return rows;
    }

    public int CreateSale(Sale newSale)
    {
        string message;
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
                sales_command.Parameters.AddWithValue("$Customer_Id", newSale.Customer_Id);
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

                    Console.WriteLine(item.Key.ToString(), item.Value);
                    total_rows_affected += sales_products_command.ExecuteNonQuery();
                }

                transaction.Commit();
                if (sales_rowsAffected > 0 & total_rows_affected > 0)
                {
                    message = "Successfully inserted data";
                }
                else
                {
                    message = "No row update";
                }
                return sales_rowsAffected;
            }
        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }
    }

    public int DeleteSale(Guid id)
    {
        string message;

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
            if (rowsAffected > 0)
            {
                message = $"Successfully deleted entry with id: {id}";
            }
            return rowsAffected;
        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            throw new ApplicationException("Database operation failed");
        }
    }
}
