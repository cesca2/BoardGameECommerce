using System.Net;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace SessionAPI.IntegrationTests;

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string DashboardPath = "/api/dashboard";

    public DashboardControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        //create clients outside of constructor since different access permissions are needed per test in this suite
        _factory = factory;
    }

    [Fact]
    public async Task Get_Dashboard_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(DashboardPath);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Dashboard_Endpoint_AsCustomer_Returns_Forbidden()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Customer");

        // Act
        var response = await client.GetAsync(DashboardPath);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_Dashboard_Endpoint_AsAdmin_ReturnsSuccesAndCorrectContentType()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Admin");

        // Act
        var response = await client.GetAsync(DashboardPath);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        if (response.Content.Headers.ContentType is not null)
        {
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString()
            );
        }
        else
        {
            Assert.Fail("Content type is empty");
        }
    }
}
