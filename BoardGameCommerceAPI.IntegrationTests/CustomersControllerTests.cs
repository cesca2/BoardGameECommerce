// Test authorisation works on protected endpoints
using System.Net;
using Microsoft.Data.Sqlite;

namespace BoardGameCommerceAPI.IntegrationTests;

public class CustomersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string CustomerInfoPath = "/api/customers/me";
    private readonly string CustomerAdminPath = "/api/customers/admin";

    public CustomersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        //create clients outside of constructor since different access permissions are needed per test in this suite
        _factory = factory;
    }

    [Fact]
    public async Task Get_CustomerInfo_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(CustomerInfoPath);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_CustomerInfo_Endpoint_AsCustomer_AuthorisationGiven_AndReturnsSuccessAndCorrectContent()
    {
        //Arrange - create authenticated customer with specific details which can be checked in assertion
        var customer_guid = Guid.NewGuid();
        var customer_email = customer_guid.ToString() + "@example.com"; // initialise email here as it needs to be unique across each test
        var client = _factory.CreateAuthenticatedClient(
            role: "Customer",
            customer_guid,
            customer_email
        );

        // Act
        var response = await client.GetAsync(CustomerInfoPath);
        var response_dto = await response.Content.ReadFromJsonAsync<CustomerDetailsDTO>();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.NotNull(response_dto);

        Assert.Equal(
            "application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString()
        );

        Assert.Equal(customer_guid.ToString(), response_dto.Id.ToString());
        Assert.Equal(customer_email.ToString(), response_dto.Email.ToString());
    }

    [Fact]
    public async Task Get_CustomerAdmin_Endpoint_AsAdmin_ReturnsSuccessAndCorrectContentType()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Admin");

        // Act
        var response = await client.GetAsync(CustomerAdminPath);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Equal(
            "application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString()
        );
    }

    [Fact]
    public async Task Get_CustomerAdmin_Endpoint_AsCustomer_Returns_Forbidden()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Customer");

        // Act
        var response = await client.GetAsync(CustomerAdminPath);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_CustomerAdmin_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(CustomerAdminPath);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_CustomerRegistration_EntersCorrectDetailsAndHashedPassword()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customer = new CreateCustomerDTO
        {
            Name = "Registration Test",
            Email = "regtest@email.com",
            Password = "regtest123",
        };

        // Act - Register customer and read from database
        var response = await client.PostAsJsonAsync("api/customers/register", customer);

        var _dbContext = _factory.Services.GetRequiredService<IDbConnectionFactory>();
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                SELECT name, email, password_hash
                FROM users
                WHERE email = $email AND role='customer';
            """;
        command.Parameters.Add(new SqliteParameter("$email", customer.Email));
        using var datareader = command.ExecuteReader();

        CreateCustomerDTO? sqlCustomer = null;
        while (datareader.Read())
        {
            sqlCustomer = new CreateCustomerDTO
            {
                Name = datareader.GetString(0),
                Email = datareader.GetString(1),
                Password = datareader.GetString(2),
            };
        }

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(sqlCustomer);
        Assert.Equal(customer.Name, sqlCustomer.Name);
        Assert.Equal(customer.Email, sqlCustomer.Email);
        Assert.DoesNotContain(customer.Password, sqlCustomer.Password); // Stored password should be hashed
    }

    [Fact]
    public async Task Post_UserLogin_CorrectPassword_Returns_OK()
    {
        // Arrange - use admin for test as are seeded and 'registered' at database startup
        var client = _factory.CreateClient();
        var config = _factory.Services.GetRequiredService<IConfiguration>();

        var customer = new LoginCustomerDTO
        {
            Email =
                config["AdminInfo:Email"] ?? throw new ApplicationException("Admin Info not found"),
            Password =
                config["AdminInfo:Password"]
                ?? throw new ApplicationException("Admin Info not found"),
        };

        // Act
        var response = await client.PostAsJsonAsync("api/customers/login", customer);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_UserLogin_IncorrectPassword_Returns_Unauthorized()
    {
        // Arrange  - use admin for test as are seeded and 'registered' at database startup
        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var client = _factory.CreateClient();
        var customer = new LoginCustomerDTO
        {
            Email =
                config["AdminInfo:Email"] ?? throw new ApplicationException("Admin Info not found"),
            Password = "fakeadminpassword",
        };

        // Act
        var response = await client.PostAsJsonAsync("api/customers/login", customer);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
