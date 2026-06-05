// Test authorisation works on protected endpoints
using System.Net;
using Microsoft.Data.Sqlite;

namespace SessionAPI.IntegrationTests;

public class CustomersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string CustomerInfoPath = "/api/customers/me";
    private readonly string CustomerAdminPath = "/api/customers/admin";

    public CustomersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
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
    public async Task Get_CustomerInfo_Endpoint_AsCustomer_AuthorisationGiven()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Customer");

        // Act
        var response = await client.GetAsync(CustomerInfoPath);

        // Assert
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_CustomerAdmin_Endpoint_AsAdmin_ReturnsSuccess()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Admin");

        // Act
        var response = await client.GetAsync(CustomerAdminPath);

        // Assert
        response.EnsureSuccessStatusCode();
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

    // TEST REGISTER, do raw sql to check details entered correctly and password hash stored not password
    [Fact]
    public async Task Post_CustomerRegistration_EntersCorrectDetailsAndHashedPassword()
    {
        var client = _factory.CreateClient();
        // Arrange
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
        Assert.NotEqual(customer.Password, sqlCustomer.Password);
    }

    [Fact]
    public async Task Post_UserLogin_CorrectPassword_Returns_OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customer = new LoginCustomerDTO
        {
            Email = "test-admin@email.com",
            Password = "testadmin123",
        };

        // Act
        var response = await client.PostAsJsonAsync("api/customers/login", customer);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_UserLogin_IncorrectPassword_Returns_Unauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customer = new LoginCustomerDTO
        {
            Email = "test-admin@email.com",
            Password = "fakeadminpassword",
        };

        // Act
        var response = await client.PostAsJsonAsync("api/customers/login", customer);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
