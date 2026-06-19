using System.Net;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace BoardGameCommerceAPI.IntegrationTests;

public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string CustomerSalesPath = "/api/sales";
    private readonly string AdminSalesPath = "/api/sales/admin";

    public SalesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        //create clients outside of constructor since different access permissions are needed per test in this suite
        _factory = factory;
    }

    [Fact]
    public async Task Create_Sale_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();
        var _testSaleDTO = _factory.SeedSaleDTO();

        // Act
        var response = await client.PostAsJsonAsync(CustomerSalesPath, _testSaleDTO);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_CustomerSales_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(CustomerSalesPath);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_SalesAdmin_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(AdminSalesPath);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_SalesAdmin_Endpoint_AsCustomer_Returns_Forbidden()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Customer");

        // Act
        var response = await client.GetAsync(AdminSalesPath);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_SalesAdmin_Endpoint_AsAdmin_ReturnsSuccess()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Admin");

        // Act
        var response = await client.GetAsync(AdminSalesPath);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_Sale_Endpoint_AsCustomer_ReturnsSuccess_And_EntersCorrectDetailsIntoDb()
    {
        //Arrange

        var customer_guid = Guid.NewGuid();
        var client = _factory.CreateAuthenticatedClient(role: "Customer", customer_guid);
        var _testSaleDTO = _factory.SeedSaleDTO();

        // Act - post test sale, read sale from database using SQL
        var response = await client.PostAsJsonAsync(CustomerSalesPath, _testSaleDTO);

        var saleResponseJsonString = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        var saleId =
            JsonSerializer
                .Deserialize<JsonElement>(saleResponseJsonString)
                .GetProperty("id")
                .GetString()
            ?? "";

        response.EnsureSuccessStatusCode();

        var _dbContext = _factory.Services.GetRequiredService<IDbConnectionFactory>();
        using var connection = _dbContext.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT sales.id, customer_id, product_id, quantity, date, time
            FROM sales JOIN sales_products ON sales.id = sales_products.sale_id ;
            """;

        command.Parameters.Add(new SqliteParameter("$id", saleId));
        using var datareader = command.ExecuteReader();

        SaleDTO? sqlSaleDTO = null;
        Guid? sqlCustomerId = null;
        while (datareader.Read())
        {
            Dictionary<Guid, int> BasketQuantitiesByProductId = [];
            BasketQuantitiesByProductId[datareader.GetGuid(2)] = datareader.GetInt16(3);

            sqlSaleDTO = new SaleDTO
            {
                QuantitiesByProductID = BasketQuantitiesByProductId,
                Date = DateOnly.Parse(datareader.GetString(4)),
                Time = TimeOnly.Parse(datareader.GetString(5)),
            };
            sqlCustomerId = datareader.GetGuid(1);
        }

        // Assert
        Assert.NotNull(sqlSaleDTO);
        Assert.Equal(_testSaleDTO.Date, sqlSaleDTO.Date);
        Assert.Equal(_testSaleDTO.Time.ToShortTimeString(), sqlSaleDTO.Time.ToShortTimeString());
        Assert.Equal(
            _testSaleDTO.QuantitiesByProductID.Values,
            sqlSaleDTO.QuantitiesByProductID.Values
        );
        Assert.Equal(customer_guid, sqlCustomerId);
    }

    [Fact]
    public async Task Get_CustomerSalesInfo_Endpoint_AsCustomer_AuthorisationGiven()
    {
        //Arrange
        var client = _factory.CreateAuthenticatedClient(role: "Customer");

        // Act
        var response = await client.GetAsync(CustomerSalesPath);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
