using System.Net.Http.Headers;

public static class CustomWebApplicationFactoryExtensions
{
    public static HttpClient CreateAuthenticatedClient(
        this CustomWebApplicationFactory<Program> factory,
        string role,
        Guid? customer_id = null
    )
    {
        var client = factory.CreateClient();

        var config = factory.Services.GetRequiredService<IConfiguration>();

        var customer = new Customer
        {
            Id = customer_id ?? Guid.NewGuid(),
            Name = "example",
            Email = "example@email" + Guid.NewGuid().ToString() + ".com",
        };

        // needs to go in to sql database for foreign key constraints
        string token = JWTTokenFactory.GenerateUserJWT(config, customer, role);

        var _dbContext = factory.Services.GetRequiredService<IDbConnectionFactory>();
        var connection = _dbContext.CreateConnection();
        connection.Open();

        var customer_command = connection.CreateCommand();

        customer_command.CommandText = """
                INSERT INTO users(id, name, email, role, password_hash)
                VALUES
                ( $Id,
                  $Name,
                  $Email,
                  $Role,
                  $PasswordHash)
                ;
            """;
        customer_command.Parameters.AddWithValue("$Id", customer.Id.ToString());
        customer_command.Parameters.AddWithValue("$Name", customer.Name);
        customer_command.Parameters.AddWithValue("$Email", customer.Email);
        customer_command.Parameters.AddWithValue("$Role", role);
        customer_command.Parameters.AddWithValue("$PasswordHash", "testpasswordhash");

        customer_command.ExecuteNonQuery();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
