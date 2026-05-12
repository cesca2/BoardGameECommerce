using Microsoft.Data.Sqlite;
using System.Globalization;


public class SaleService : ISaleService
{
    private readonly IDbConnectionFactory _dbContext;

    public SaleService(IDbConnectionFactory dbContext)
    {
        _dbContext = dbContext;

    }
    /*
    public List<Sale>? GetAllSales()
    {

        List<Sale> rows = new ();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT sales.id, customer_id FROM sales JOIN sales_products ON
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
                    rows.Add(new Sale
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0)
                        });
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

    public Sale? GetSale(Guid id)
    {

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email FROM Sales 
            WHERE id = $id;
        """;
        command.Parameters.Add(new SqliteParameter("$id", id.ToString()));

        try{
            using var datareader = command.ExecuteReader();

        if (!datareader.HasRows) return null;
        else
        {
            while (datareader.Read())
            {
                {

                        var sale = new Sale
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0)
                        };
                        return sale;
                }
            }
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
    */
    
    public Sale CreateSale(Sale newSale)
    {

        string message;
        try
        {

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using (var transaction = connection.BeginTransaction())
        {
        using var sales_command = connection.CreateCommand();
        sales_command.CommandText =
        """
                INSERT INTO sales(id, customer_id) 
                VALUES 
                ( $Id,
                  $Customer_Id )
                ;
            """;
        
        sales_command.Parameters.AddWithValue("$Id", newSale.Id.ToString());
        sales_command.Parameters.AddWithValue("$Customer_Id", newSale.Customer_Id);
        
        int sales_rowsAffected = sales_command.ExecuteNonQuery();

        using var sales_products_command = connection.CreateCommand();
        sales_products_command.CommandText =
        """
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

        foreach(var item in newSale.QuantitiesByProductID)
            {
                sales_products_command.Parameters.Clear();
                sales_products_command.Parameters.AddWithValue("$Id", Guid.NewGuid().ToString());
                sales_products_command.Parameters.AddWithValue("$Sale_Id", newSale.Id.ToString());
                sales_products_command.Parameters.AddWithValue("$Product_Id", item.Key.ToString()); 
                sales_products_command.Parameters.AddWithValue("$Quantity", item.Value);
                
                Console.WriteLine(item.Key.ToString(), item.Value);
                total_rows_affected += sales_products_command.ExecuteNonQuery();

            }
        
        transaction.Commit();
        if (sales_rowsAffected > 0 & total_rows_affected>0 )
            {
                message = "Successfully inserted data";
            }
            else
            {
                message = "No row update";
            }
        }
        
        }
       
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }


        return newSale;

    }

    public string? DeleteSale(Guid id)
    {
        string message;

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
        """
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
            else
            {
                return null;
            }

        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            throw new ApplicationException("Database operation failed");
        }

        return message;

    }

}