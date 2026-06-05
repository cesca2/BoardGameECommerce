using System.Net.Http.Headers;

public static class CustomWebApplicationFactoryExtensions
{
    public static SaleDTO SeedSaleDTO(this CustomWebApplicationFactory<Program> factory)
    {
        Product TestProduct = new Product
        {
            Name = "TestProduct",
            Price = 25,
            YearPublished = 2015,
        };
        var _dbContext = factory.Services.GetRequiredService<IDbConnectionFactory>();
        var connection = _dbContext.CreateConnection();
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO products(id, name, yearpublished, price)
            VALUES
            ( $Id,
              $Name,
              $Year,
              $Price)
            ;
            """;
        command.Parameters.AddWithValue("$Id", TestProduct.Id.ToString());
        command.Parameters.AddWithValue("$Name", TestProduct.Name);
        command.Parameters.AddWithValue("$Year", TestProduct.YearPublished);
        command.Parameters.AddWithValue("$Price", TestProduct.Price);
        command.ExecuteNonQuery();

        Dictionary<Guid, int> BasketQuantitiesByProductId = [];
        BasketQuantitiesByProductId[TestProduct.Id] = 1;

        return new SaleDTO
        {
            QuantitiesByProductID = BasketQuantitiesByProductId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
        };
    }

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
