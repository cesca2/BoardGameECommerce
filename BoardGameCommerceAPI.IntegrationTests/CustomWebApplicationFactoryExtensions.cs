using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using AutoFixture;

public static class CustomWebApplicationFactoryExtensions
{
    public static SaleDTO SeedSaleDTO(this CustomWebApplicationFactory<Program> factory)
    {
        Product TestProduct = SeedProduct(factory);

        // create sale DTO to use in tests using mock product's id
        Dictionary<Guid, int> BasketQuantitiesByProductId = [];
        BasketQuantitiesByProductId[TestProduct.Id] = 1;

        var fixture = new Fixture();

        fixture.Customize<SaleDTO>(transform =>
            transform
                .With(dto => dto.QuantitiesByProductID, BasketQuantitiesByProductId)
                .With(dto => dto.Date, DateOnly.FromDateTime(DateTime.Now))
        );
        return fixture.Create<SaleDTO>();
    }

    public static Product SeedProduct(this CustomWebApplicationFactory<Program> factory)
    {
        // Create mock product which is needed to be pre-inserted into products table
        var fixture = new Fixture();
        Product TestProduct = fixture.Create<Product>();

        var _dbContext = factory.Services.GetRequiredService<IDbConnectionFactory>();
        using var connection = _dbContext.CreateConnection();
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

        return TestProduct;
    }

    public static HttpClient CreateAuthenticatedClient(
        this CustomWebApplicationFactory<Program> factory,
        string role,
        Guid? customer_id = null,
        string? email = null
    )
    {
        var client = factory.CreateClient();

        var config = factory.Services.GetRequiredService<IConfiguration>();

        var fixture = new Fixture();

        fixture.Customize<Customer>(transform =>
            transform
                .With(dto => dto.Id, customer_id ?? Guid.NewGuid())
                .With(
                    dto => dto.Email,
                    email ?? "example@email" + Guid.NewGuid().ToString() + ".com"
                )
        );

        var customer = fixture.Create<Customer>();

        // needs to go in to sql database for foreign key constraints
        string token = JWTTokenFactory.GenerateUserJWT(config, customer, role);

        var _dbContext = factory.Services.GetRequiredService<IDbConnectionFactory>();
        using var connection = _dbContext.CreateConnection();
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
        customer_command.Parameters.AddWithValue("$Role", role.ToLower());
        customer_command.Parameters.AddWithValue("$PasswordHash", customer.PasswordHash);

        customer_command.ExecuteNonQuery();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
