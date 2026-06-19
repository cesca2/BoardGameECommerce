using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Routing;
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

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(4, 3)]
    [InlineData(4, 11)]
    public async Task Get_Dashboard_Endpoint_AsAdmin_WithProvidedSalesAndCustomersNumbers_ReturnsSuccesAndCorrectEntries(
        int expectedCustomers,
        int expectedSales
    )
    {
        //Arrange

        // reinitialise database for consecutive tests to run on clean environment
        using (var scope = _factory.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            var reinit_setting = scope.ServiceProvider.GetRequiredService<IConfiguration>()[
                "ReInitialize"
            ];

            initializer.Initialize(reInitialize: true);
        }

        List<Guid> customers = new List<Guid>();
        List<Guid> salesproducts = new List<Guid>();

        // set no limitation on length of bestsellers list for this test
        var url = DashboardPath + "?bestsellerLimit=-1";

        int i = 0;
        while (i < expectedCustomers)
        {
            var customer_id = Guid.NewGuid();
            _factory.CreateAuthenticatedClient(role: "Customer", customer_id: customer_id);
            i++;
            customers.Add(customer_id);
        }

        i = 0;
        while (i < expectedSales)
        {
            var random = new Random();

            var sale = _factory.SeedSale(customers.OrderBy(x => random.Next()).Take(1).ToList()[0]);
            i++;
            salesproducts.Add(sale.QuantitiesByProductID.Keys.ToList()[0]);
        }

        // Admins shouldn't affect the overall customer count
        var client = _factory.CreateAuthenticatedClient(role: "Admin");

        // Act
        var response = await client.GetAsync(DashboardPath);
        response.EnsureSuccessStatusCode();

        var items = await client.GetFromJsonAsync<DashboardDTO>(url);
        if (items is null)
        {
            items = new DashboardDTO
            {
                NumCustomersTotal = -1,
                NumSalesTotal = -1,
                Bestsellers = [],
            };
        }

        // Assert
        Assert.Equal(expectedCustomers, items.NumCustomersTotal);
        Assert.Equal(expectedSales, items.NumSalesTotal);
        i = 0;
        while (i < salesproducts.Count)
        {
            Assert.Contains(
                salesproducts[i],
                items.Bestsellers.Select(bestseller => bestseller.Id).ToList()
            );
            i++;
        }
    }
}
