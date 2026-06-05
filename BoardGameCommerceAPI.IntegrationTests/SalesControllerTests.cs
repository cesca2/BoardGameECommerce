using System.Net;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace SessionAPI.IntegrationTests;

public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string CustomerSalesPath = "/api/sales";
    private readonly string AdminSalesPath = "/api/sales/admin";
    private readonly Product testproduct = new Product
    {
        Id = Guid.NewGuid(),
        Name = "TestProduct",
        Price = 25,
        YearPublished = 2015,
    };
    private readonly SaleDTO testsale;

    public SalesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;

        var _dbContext = _factory.Services.GetRequiredService<IDbConnectionFactory>();
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
        command.Parameters.AddWithValue("$Id", testproduct.Id.ToString());
        command.Parameters.AddWithValue("$Name", testproduct.Name);
        command.Parameters.AddWithValue("$Year", testproduct.YearPublished);
        command.Parameters.AddWithValue("$Price", testproduct.Price);
        command.ExecuteNonQuery();

        Dictionary<Guid, int> BasketQuantitiesByProductId = [];
        BasketQuantitiesByProductId[testproduct.Id] = 1;

        testsale = new SaleDTO
        {
            QuantitiesByProductID = BasketQuantitiesByProductId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
        };
    }

    [Fact]
    public async Task Create_Sale_Endpoint_NoAuthentication_Returns_Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(CustomerSalesPath, testsale);

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

        // Act - post test sale, read sale from database using SQL
        var response = await client.PostAsJsonAsync(CustomerSalesPath, testsale);

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
        Assert.Equal(testsale.Date, sqlSaleDTO.Date);
        //Assert.Equal(testsale.Time, sqlSaleDTO.Time); - needs fix to format
        Assert.Equal(
            testsale.QuantitiesByProductID.Values,
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
