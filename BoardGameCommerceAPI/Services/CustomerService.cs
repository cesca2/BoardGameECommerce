using Microsoft.Data.Sqlite;
using System.Globalization;


public class CustomerService : ICustomerService
{
    private readonly IDbConnectionFactory _dbContext;

    public CustomerService(IDbConnectionFactory dbContext)
    {
        _dbContext = dbContext;

    }

    public List<Customer>? GetAllCustomers()
    {

        List<Customer> rows = new ();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email FROM customers
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
                    rows.Add(new Customer
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

    public Customer? GetCustomerById(Guid id)
    {

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email FROM Customers 
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

                        var customer = new Customer
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0)
                        };
                        return customer;
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
    public Customer? GetCustomerByEmail(string email)
    {

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email FROM Customers 
            WHERE email = $email;
        """;
        command.Parameters.Add(new SqliteParameter("$email", email));

        try{
            using var datareader = command.ExecuteReader();

        if (!datareader.HasRows) return null;
        else
        {
            while (datareader.Read())
            {
                {

                        var customer = new Customer
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0)
                        };
                        return customer;
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

    
    public Customer CreateCustomer(Customer newCustomer)
    {
       
        string message;

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
        """
                INSERT INTO customers(id, name, email) 
                VALUES 
                ( $Id,
                  $Name, 
                  $Email)
                ;
            """;
        
        command.Parameters.AddWithValue("$Id", newCustomer.Id.ToString());
        command.Parameters.AddWithValue("$Name", newCustomer.Name);
        command.Parameters.AddWithValue("$Email", newCustomer.Email);

        try
        {
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                message = "Successfully inserted row";
            }
            else
            {
                message = "No row update";
            }

        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            Console.WriteLine(message);
            throw new ApplicationException("Database operation failed");
        }

        return newCustomer;

    }
    public string? DeleteCustomer(Guid id)
    {
        string message;

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
        """
                DELETE FROM customers
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

    public string? UpdateCustomer(Guid id, Customer newCustomer)
    {
        string message;

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
        """
        UPDATE customers
        SET 
        name = $Name, 
        email = $Email
        WHERE id = $Id
        ;
        """;
        command.Parameters.AddWithValue("$Id", newCustomer.Id.ToString());
        command.Parameters.AddWithValue("$Name", newCustomer.Name);
        command.Parameters.AddWithValue("$Email", newCustomer.Email);


        try
        {
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                message = $"Successfully updated entry with id: {id}";
            }
            else
            {
                return null;
            }

        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            throw new ApplicationException($"Database operation failed: {message}");
        }

        return message;

    }
}