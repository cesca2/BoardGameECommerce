// Test authorisation works on protected endpoints
using System.Net;

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
}
