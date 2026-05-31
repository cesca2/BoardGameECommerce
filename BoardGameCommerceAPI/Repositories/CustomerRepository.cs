using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _dbContext;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(IDbConnectionFactory dbContext, ILogger<CustomerRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public List<Customer>? GetAllCustomers()
    {
        List<Customer> rows = new();

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT id, name, email
                FROM users
                WHERE role='customer';
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
                            new Customer
                            {
                                Name = datareader.GetString(1),
                                Email = datareader.GetString(2),
                                Id = datareader.GetGuid(0),
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

    public Customer? GetCustomerById(Guid id)
    {
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT id, name, email
                FROM users
                WHERE id = $id AND role='customer';
            """;
        command.Parameters.Add(new SqliteParameter("$id", id.ToString()));

        try
        {
            using var datareader = command.ExecuteReader();

            if (!datareader.HasRows)
                return null;
            else
            {
                while (datareader.Read())
                {
                    {
                        var customer = new Customer
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0),
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
                SELECT id, name, email, password_hash
                FROM users
                WHERE email = $email AND role='customer';
            """;
        command.Parameters.Add(new SqliteParameter("$email", email));

        try
        {
            using var datareader = command.ExecuteReader();

            if (!datareader.HasRows)
                return null;
            else
            {
                while (datareader.Read())
                {
                    {
                        var customer = new Customer
                        {
                            Name = datareader.GetString(1),
                            Email = datareader.GetString(2),
                            Id = datareader.GetGuid(0),
                            PasswordHash = datareader.GetString(3),
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

    public int CreateCustomer(Customer newCustomer)
    {
        string message;

        using var connection = _dbContext.CreateConnection();
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

        command.Parameters.AddWithValue("$Id", newCustomer.Id.ToString());
        command.Parameters.AddWithValue("$Name", newCustomer.Name);
        command.Parameters.AddWithValue("$Email", newCustomer.Email);
        command.Parameters.AddWithValue("$PasswordHash", newCustomer.PasswordHash);

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
            _logger.LogInformation(message);
            return rowsAffected;
        }
        catch (SqliteException ex)
        {
            message = "SQLite Error" + ex.Message;
            _logger.LogError(ex, ex.Message);
            throw new ApplicationException("Database operation failed");
        }
    }

    public string? DeleteCustomer(Guid id)
    {
        string message;

        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                    DELETE FROM users
                    WHERE id = $ID AND role='customer'
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
        command.CommandText = """
            UPDATE users
            SET
            name = $Name,
            email = $Email
            WHERE id = $Id AND role='customer'
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
